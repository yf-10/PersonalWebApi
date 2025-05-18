using System.Threading.Channels;

namespace PersonalWebApi.Utilities;

/// <summary>
/// Provides a custom file logger provider for ILogger.
/// This provider writes log messages to a file asynchronously using a background worker and a channel.
/// Supports log level filtering and log file rotation based on file size.
/// </summary>
public class FileLoggerProvider : ILoggerProvider {
    // Path to the log file
    private readonly string filePath;
    // Minimum log level to write
    private readonly LogLevel minLogLevel;
    // Maximum file size in bytes before rotating the log file
    private readonly long maxFileSizeBytes;
    // Channel for queuing log messages for asynchronous writing
    private readonly Channel<string> logChannel = Channel.CreateUnbounded<string>();
    // Cancellation token source for background worker cancellation
    private readonly CancellationTokenSource cts = new();
    // Background worker task for writing log messages
    private readonly Task worker;
    // Indicates whether the provider has been disposed
    private volatile bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the FileLoggerProvider.
    /// </summary>
    /// <param name="filePath">Path to the log file.</param>
    /// <param name="minLogLevel">Minimum log level to write.</param>
    /// <param name="maxFileSizeBytes">Maximum file size in bytes before rotation.</param>
    public FileLoggerProvider(string filePath, LogLevel minLogLevel = LogLevel.Information, long maxFileSizeBytes = 10 * 1024 * 1024) {
        this.filePath = filePath;
        this.minLogLevel = minLogLevel;
        this.maxFileSizeBytes = maxFileSizeBytes;
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        worker = Task.Run(WriteLogWorkerAsync);
    }

    /// <summary>
    /// Creates a new FileLogger instance for the specified category.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>A FileLogger instance.</returns>
    public ILogger CreateLogger(string categoryName) {
        return new FileLogger(categoryName, minLogLevel, logChannel, () => disposed);
    }

    /// <summary>
    /// Disposes the logger provider and releases all resources.
    /// Signals the background worker to complete and waits for it to finish.
    /// </summary>
    public void Dispose() {
        if (disposed) return;
        disposed = true;
        logChannel.Writer.Complete();
        try { worker.Wait(); } catch { }
        cts.Cancel();
        cts.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Background worker that writes log messages from the channel to the file asynchronously.
    /// Handles cancellation and exceptions robustly.
    /// </summary>
    private async Task WriteLogWorkerAsync() {
        try {
            await foreach (var msg in logChannel.Reader.ReadAllAsync(cts.Token)) {
                try {
                    await SafeRotateAndWriteAsync(msg);
                } catch (IOException ioEx) {
                    Console.Error.WriteLine($"FileLogger IO Error: {ioEx.Message}");
                } catch (Exception ex) {
                    Console.Error.WriteLine($"FileLogger Error: {ex.Message}");
                }
            }
        } catch (OperationCanceledException) {
            // Graceful shutdown on cancellation
        } catch (Exception ex) {
            Console.Error.WriteLine($"FileLogger Worker Fatal Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Safely rotates the log file if needed and writes the log message asynchronously.
    /// Retries on IO exceptions.
    /// </summary>
    /// <param name="msg">The log message to write.</param>
    private async Task SafeRotateAndWriteAsync(string msg) {
        for (int retry = 0; retry < 3; retry++) {
            try {
                await RotateIfNeededAsync();
                using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, useAsync: true);
                using var sw = new StreamWriter(fs);
                await sw.WriteLineAsync(msg);
                break;
            } catch (IOException) {
                await Task.Delay(100);
            }
        }
    }

    /// <summary>
    /// Rotates the log file if it exceeds the maximum allowed size.
    /// Uses exclusive lock to avoid race conditions.
    /// </summary>
    private async Task RotateIfNeededAsync() {
        if (!File.Exists(filePath)) return;
        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length < maxFileSizeBytes) return;

        string dir = Path.GetDirectoryName(filePath) ?? ".";
        string name = Path.GetFileNameWithoutExtension(filePath);
        string ext = Path.GetExtension(filePath);
        string archiveName = $"{name}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
        string archivePath = Path.Combine(dir, archiveName);
        int retry = 3;
        while (retry-- > 0) {
            try {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) {
                    fs.Close();
                }
                File.Move(filePath, archivePath, overwrite: false);
                break;
            } catch (IOException) {
                await Task.Delay(100);
            }
        }
    }
}

/// <summary>
/// Custom file logger implementation using a background channel.
/// This logger writes log messages to a channel, which are then processed asynchronously by a background worker.
/// </summary>
public class FileLogger(
    string categoryName,
    LogLevel minLogLevel,
    Channel<string> logChannel,
    Func<bool> isDisposed
) : ILogger {
    // The category name for the logger (e.g., class or feature name)
    private readonly string categoryName = categoryName;
    // The minimum log level to write
    private readonly LogLevel minLogLevel = minLogLevel;
    // The channel used to queue log messages for asynchronous writing
    private readonly Channel<string> logChannel = logChannel;
    // Function to check if the provider has been disposed
    private readonly Func<bool> isDisposed = isDisposed;

    /// <summary>
    /// NullScope is a no-op disposable used for BeginScope.
    /// </summary>
    private class NullScope : IDisposable {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }
    }

    /// <summary>
    /// Returns a no-op scope object.
    /// </summary>
    IDisposable ILogger.BeginScope<TState>(TState state) => NullScope.Instance;

    /// <summary>
    /// Checks if the given log level is enabled.
    /// </summary>
    public bool IsEnabled(LogLevel logLevel) => logLevel >= minLogLevel;

    /// <summary>
    /// Writes a log entry to the channel if enabled and not disposed.
    /// </summary>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        if (!IsEnabled(logLevel) || isDisposed()) return;
        var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] [{categoryName}] {formatter(state, exception)}";
        if (exception != null) {
            msg += $" {exception}";
        }
        try {
            logChannel.Writer.TryWrite(msg);
        } catch {
            // Channel is completed/disposed, ignore
        }
    }
}