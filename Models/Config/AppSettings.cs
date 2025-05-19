namespace PersonalWebApi.Models.Config;

/// <summary>
/// appsettings.jsonの内容をバインドするPOCOクラス
/// </summary>
public class AppSettings
{
    public LoggingSettings Logging { get; set; } = new();
    public ConnectionStringsSettings ConnectionStrings { get; set; } = new();
}

public class LoggingSettings
{
    public LogLevelSettings LogLevel { get; set; } = new();
}

public class LogLevelSettings
{
    // public string Default { get; set; } = "Information";
    public string MicrosoftAspNetCore { get; set; } = "Warning";
}

public class ConnectionStringsSettings
{
    public string Postgres { get; set; } = "";
}