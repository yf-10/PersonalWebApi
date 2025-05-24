namespace PersonalWebApi.Models.Config;

/// <summary>
/// appsettings.jsonの内容をバインドするPOCOクラス
/// </summary>
public class AppSettings {
    public LoggingSettings Logging { get; set; } = new();
    public bool IsTest { get; set; } = true;
    public ConnectionStringsSettings ConnectionStrings { get; set; } = new();
    public Api Api { get; set; } = new();
}

public class LoggingSettings {
    public LogLevelSettings LogLevel { get; set; } = new();
}

public class LogLevelSettings {
    public string Default { get; set; } = "Information";
    public string MicrosoftAspNetCore { get; set; } = "Warning";
}

public class ConnectionStringsSettings {
    public string Postgres { get; set; } = "";
}

public class Api {
    public string ApiKey { get; set; } = "e74c2f8a-3d92-4a67-95e2-8c874baf37db";
    public string ApiKeyHeaderName { get; set; } = "X-Api-Key";
}