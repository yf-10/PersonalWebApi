using System.Threading.Channels;

namespace PersonalWebApi.Utilities;
/// --------------------------------------------------------------------------------
/// <summary>
/// ファイルへの非同期ロギングを提供するILoggerProvider実装
/// バックグラウンドワーカーとチャネルを使い、アプリのパフォーマンスを損なわずにログをファイルへ書き込む
/// ログレベルによるフィルタリングやファイルサイズによる自動ローテーションもサポート
/// </summary>
/// --------------------------------------------------------------------------------
public class FileLoggerProvider : ILoggerProvider {
    private readonly string filePath;
    private readonly LogLevel minLogLevel;
    private readonly long maxFileSizeBytes;
    private readonly Channel<string> logChannel = Channel.CreateUnbounded<string>();
    private readonly CancellationTokenSource cts = new();
    private readonly Task worker;
    private volatile bool disposed = false;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// ファイルロガープロバイダー：初期化
    /// </summary>
    /// <param name="filePath">ログファイルのパス</param>
    /// <param name="minLogLevel">出力する最小ログレベル</param>
    /// <param name="maxFileSizeBytes">ローテーション前の最大ファイルサイズ（Byte）</param>
    /// --------------------------------------------------------------------------------
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

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// ファイルロガー：インスタンス生成
    /// </summary>
    /// <param name="categoryName">ロガーのカテゴリ名（通常はクラス名）</param>
    /// <returns>FileLogger インスタンス</returns>
    /// --------------------------------------------------------------------------------
    public ILogger CreateLogger(string categoryName) {
        return new FileLogger(categoryName, minLogLevel, logChannel, () => disposed);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// プロバイダーを破棄し、バックグラウンドワーカーの終了とリソース解放を行う
    /// タイムアウト（例: 5秒）を超えた場合は強制的に終了する
    /// </summary>
    /// --------------------------------------------------------------------------------
    public void Dispose() {
        if (disposed) return;
        disposed = true;
        logChannel.Writer.Complete();
        try {
            // タイムアウトを5秒に設定
            if (!worker.Wait(TimeSpan.FromSeconds(5))) {
                Console.Error.WriteLine("FileLoggerProvider: ログ書き込みワーカーの終了待機がタイムアウトしました");
            }
        } catch { }
        cts.Cancel();
        cts.Dispose();
        GC.SuppressFinalize(this);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// チャネルからログメッセージを受信し、ファイルへ非同期で書き込むバックグラウンドワーカー
    /// IO例外時はリトライし、キャンセルや致命的例外も安全に処理する
    /// </summary>
    /// --------------------------------------------------------------------------------
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
            // キャンセル時は正常終了
        } catch (Exception ex) {
            Console.Error.WriteLine($"FileLogger Worker Fatal Error: {ex.Message}");
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// ファイルサイズが上限を超えていればローテーションし、ログメッセージを書き込む
    /// IO例外時はリトライする
    /// </summary>
    /// <param name="msg">書き込むログメッセージ</param>
    /// --------------------------------------------------------------------------------
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

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// ファイルサイズが最大値を超えていれば、タイムスタンプ付きのファイル名でローテーションする
    /// 排他制御を行い、IO例外時はリトライする
    /// </summary>
    /// --------------------------------------------------------------------------------
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

/// --------------------------------------------------------------------------------
/// <summary>
/// バックグラウンドチャネルを使い、非同期でファイルに書き込むカスタムILogger実装
/// ログレベルによるフィルタリングやカテゴリ名付与、例外情報の付加も行う
/// </summary>
/// <remarks>
/// ファイルロガー：初期化
/// </remarks>
/// <param name="categoryName">ロガーのカテゴリ名</param>
/// <param name="minLogLevel">出力する最小ログレベル</param>
/// <param name="logChannel">ログメッセージ送信用チャネル</param>
/// <param name="isDisposed">プロバイダー破棄済み判定デリゲート</param>
/// --------------------------------------------------------------------------------
public class FileLogger(string categoryName, LogLevel minLogLevel, Channel<string> logChannel, Func<bool> isDisposed) : ILogger {
    private readonly string categoryName = categoryName;
    private readonly LogLevel minLogLevel = minLogLevel;
    private readonly Channel<string> logChannel = logChannel;
    private readonly Func<bool> isDisposed = isDisposed;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// NullScopeはBeginScope用の何もしないIDisposable
    /// </summary>
    /// --------------------------------------------------------------------------------
    private class NullScope : IDisposable {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// スコープロギングは未対応のため、何もしないスコープを返す
    /// </summary>
    /// --------------------------------------------------------------------------------
    IDisposable ILogger.BeginScope<TState>(TState state) => NullScope.Instance;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 指定したログレベルが有効かどうかを判定する
    /// </summary>
    /// --------------------------------------------------------------------------------
    public bool IsEnabled(LogLevel logLevel) => logLevel >= minLogLevel;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 有効かつ未破棄の場合、チャネルにログエントリを書き込む
    /// ログにはタイムスタンプ、レベル、カテゴリ、例外情報も付加される
    /// </summary>
    /// --------------------------------------------------------------------------------
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        if (!IsEnabled(logLevel) || isDisposed()) return;
        var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] [{categoryName}] {formatter(state, exception)}";
        if (exception != null) msg += $" {exception}";
        try {
            logChannel.Writer.TryWrite(msg);
        } catch {
            // チャネルが完了/破棄済みの場合は無視
        }
    }
}