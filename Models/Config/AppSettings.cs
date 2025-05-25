namespace PersonalWebApi.Models.Config;
/// --------------------------------------------------------------------------------
/// <summary>
/// "appsettings.json" をバインドするPOCOクラス
/// </summary>
/// --------------------------------------------------------------------------------
public class AppSettings {
    public Logging Logging { get; set; } = new();
    public bool IsTest { get; set; } = true;
    public string ApplicationName { get; set; } = "PersonalWebApi";
    public string Version { get; set; } = "1.0.0";
    public CustomFileLogger CustomFileLogger { get; set; } = new();
    public ApiSettings ApiSettings { get; set; } = new();
    public Database Database { get; set; } = new();
    public GmailApi GmailApi { get; set; } = new();
}
/// <summary>
/// 標準ロガー設定
/// </summary>
public class Logging {
    public LogLevel LogLevel { get; set; } = new();
}
/// <summary>
/// 標準ロガー設定 > ログレベル
/// </summary>
public class LogLevel {
    public string Default { get; set; } = "Information";
    public string MicrosoftAspNetCore { get; set; } = "Warning";
}
/// <summary>
/// 標準ロガー設定 > ファイル出力設定
/// </summary>
public class FileOutput {
    public bool Enabled { get; set; } = true;
    public string FilePath { get; set; } = "./logs/app.log";
    public long MaxFileSizeKB { get; set; } = 10 * 1024; // 10MB
}
/// <summary>
/// カスタムファイルロガー設定
/// </summary>
public class CustomFileLogger {
    public ResponseLogger ResponseLogger { get; set; } = new();
}
/// <summary>
/// カスタムファイルロガー設定 > レスポンスロガー
/// </summary>
public class ResponseLogger {
    public bool Enabled { get; set; } = false;
    public string FilePath { get; set; } = "./logs/custom_file_logger.log";
    public long MaxFileSizeKB { get; set; } = 10 * 1024; // 1MB
}
/// <summary>
/// API設定
/// </summary>
public class ApiSettings {
    public string ApiKey { get; set; } = "";
    public string ApiKeyHeaderName { get; set; } = "X-Api-Key";
}
/// <summary>
/// データベース接続文字列設定
/// </summary>
public class Database {
    public string PostgresConStr { get; set; } = "";
}
/// <summary>
/// Gmail API設定
/// </summary>
public class GmailApi {
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string TargetLabelName { get; set; } = "INBOX"; // 取得対象のラベル名
    public int MaxResults { get; set; } = 10; // 取得するメールの最大件数
}