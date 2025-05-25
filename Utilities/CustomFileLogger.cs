using System.Text;
using System.Threading.Channels;

namespace PersonalWebApi.Utilities;
/// --------------------------------------------------------------------------------
/// <summary>
/// ファイルに非同期でログを書き込むカスタムロガークラス
/// バックグラウンドワーカーとチャネルを利用し、アプリのパフォーマンスを損なわずにログ出力を行う
/// ファイルサイズによる自動ローテーションもサポート
/// </summary>
/// --------------------------------------------------------------------------------
public class CustomFileLogger : IDisposable {

    public enum LogLevel { ERROR = 0, WARN = 1, INFO = 2, DEBUG = 3 }
    private readonly LogLevel logLevel;
    private readonly string filePath;
    private readonly long maxFileSizeKB;
    private readonly Channel<string> logChannel = Channel.CreateUnbounded<string>();
    private readonly CancellationTokenSource cts = new();
    private readonly Task worker;
    private volatile bool disposed = false;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="filePath">ログファイルのパス</param>
    /// <param name="logLevel">出力する最小ログレベル</param>
    /// <param name="maxFileSizeKB">ローテーション前の最大ファイルサイズ（KB単位）</param>
    /// --------------------------------------------------------------------------------
    public CustomFileLogger(string filePath, LogLevel logLevel = LogLevel.INFO, long maxFileSizeKB = 1024) {
        this.logLevel = logLevel;
        this.filePath = filePath;
        this.maxFileSizeKB = maxFileSizeKB;
        var dir = Path.GetDirectoryName(filePath);
        // ログディレクトリが存在しない場合は作成
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        // バックグラウンドワーカー開始
        worker = Task.Run(WriteLogWorkerAsync);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// エラーログ出力
    /// </summary>
    /// --------------------------------------------------------------------------------
    public void Error(string msg) {
        if (LogLevel.ERROR <= logLevel)
            Out(LogLevel.ERROR, msg);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 例外情報をエラーログとして出力
    /// </summary>
    /// --------------------------------------------------------------------------------
    public void Error(Exception ex) {
        if (LogLevel.ERROR <= logLevel)
            Out(LogLevel.ERROR, ex.Message + Environment.NewLine + ex.StackTrace);
        if (ex.InnerException != null) {
            Out(LogLevel.ERROR, "Inner Exception: " + ex.InnerException.Message);
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 警告ログ出力
    /// </summary>
    /// --------------------------------------------------------------------------------
    public void Warn(string msg) {
        if (LogLevel.WARN <= logLevel)
            Out(LogLevel.WARN, msg);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 情報ログ出力
    /// </summary>
    /// --------------------------------------------------------------------------------
    public void Info(string msg) {
        if (LogLevel.INFO <= logLevel)
            Out(LogLevel.INFO, msg);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// デバッグログ出力
    /// </summary>
    /// --------------------------------------------------------------------------------
    public void Debug(string msg) {
        if (LogLevel.DEBUG <= logLevel)
            Out(LogLevel.DEBUG, msg);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// ログメッセージをチャネルに書き込む（フォーマット付き）
    /// チャネルが詰まって書き込めない場合は標準エラー出力に通知
    /// </summary>
    /// <param name="level">ログレベル</param>
    /// <param name="msg">ログメッセージ</param>
    /// --------------------------------------------------------------------------------
    private void Out(LogLevel level, string msg) {
        if (!disposed) {
            string line = string.Format(
                "[{0:yyyy-MM-dd HH:mm:ss.fff}][{1}][{2}] {3}",
                DateTime.Now,
                level.ToString().PadRight(5),
                Environment.CurrentManagedThreadId.ToString().PadLeft(4, ' '),
                msg
            );
            // チャネルが詰まっていた場合は通知
            if (!logChannel.Writer.TryWrite(line)) {
                Console.Error.WriteLine("CustomFileLogger: チャネルが詰まっているためログを破棄しました: " + line);
            }
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// チャネルに直接書き込む
    /// チャネルが詰まって書き込めない場合は標準エラー出力に通知
    /// </summary>
    /// --------------------------------------------------------------------------------
    public void WriteLine(string line) {
        if (!disposed) {
            if (!logChannel.Writer.TryWrite(line)) {
                Console.Error.WriteLine("CustomFileLogger: チャネルが詰まっているためログを破棄しました: " + line);
            }
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バックグラウンドワーカーでチャネルからログを受け取り、ファイルに非同期で書き込む
    /// </summary>
    /// --------------------------------------------------------------------------------
    private async Task WriteLogWorkerAsync() {
        try {
            await foreach (var msg in logChannel.Reader.ReadAllAsync(cts.Token)) {
                try {
                    await SafeRotateAndWriteAsync(msg);
                } catch (IOException ioEx) {
                    Console.Error.WriteLine($"CustomFileLogger IO Error: {ioEx.Message}");
                } catch (Exception ex) {
                    Console.Error.WriteLine($"CustomFileLogger Error: {ex.Message}");
                }
            }
        } catch (OperationCanceledException) {
            // 終了時
        } catch (Exception ex) {
            Console.Error.WriteLine($"CustomFileLogger Worker Fatal Error: {ex.Message}");
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// ファイルサイズが上限を超えていればローテーションし、ログメッセージを書き込む
    /// </summary>
    /// --------------------------------------------------------------------------------
    private async Task SafeRotateAndWriteAsync(string msg) {
        for (int retry = 0; retry < 3; retry++) {
            try {
                await RotateIfNeededAsync();
                using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, useAsync: true);
                using var sw = new StreamWriter(fs, Encoding.UTF8);
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
    /// </summary>
    /// --------------------------------------------------------------------------------
    private async Task RotateIfNeededAsync() {
        if (!File.Exists(filePath)) return;
        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length < maxFileSizeKB * 1024) return; // KB単位で比較
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

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// ファイナライザ（ガベージコレクション時のリソース解放）
    /// </summary>
    /// --------------------------------------------------------------------------------
    ~CustomFileLogger() {
        Dispose(false);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// IDisposable 実装
    /// </summary>
    /// --------------------------------------------------------------------------------
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// Disposeパターン
    /// </summary>
    /// <param name="disposing">true:明示的解放, false:ファイナライザから</param>
    /// --------------------------------------------------------------------------------
    protected virtual void Dispose(bool disposing) {
        if (disposed) return;
        disposed = true;
        if (disposing) {
            // マネージドリソースの解放
            logChannel.Writer.Complete();
            try {
                if (!worker.Wait(TimeSpan.FromSeconds(5))) {
                    Console.Error.WriteLine("CustomFileLogger: ログ書き込みワーカーの終了待機がタイムアウトしました");
                }
            } catch { }
            cts.Cancel();
            cts.Dispose();
        }
        // アンマネージドリソースがあればここで解放
    }

}
