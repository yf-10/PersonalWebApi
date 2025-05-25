using System.Diagnostics;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using PersonalWebApi.Models.Config;

namespace PersonalWebApi.Controllers;
/// --------------------------------------------------------------------------------
/// <summary>
/// コントローラー基底クラス
/// </summary>
/// <param name="logger">標準ロガー</param>
/// <param name="options">アプリケーション設定オプション</param>
/// --------------------------------------------------------------------------------
public abstract class BaseController(ILogger logger, IOptionsSnapshot<AppSettings> options) : ControllerBase, IActionFilter {

    /// <summary>
    /// 標準ロガー
    /// </summary>
    protected readonly ILogger _logger = logger;

    /// <summary>
    /// アプリケーション設定オプション
    /// </summary>
    protected readonly IOptions<AppSettings> _options = options;

    /// <summary>
    /// 処理時間計測用のストップウォッチキー
    /// </summary>
    private const string StopwatchKey = "_BaseAuthenticatedController_Stopwatch";

    /// <summary>
    /// 処理時間ログ用のカスタムファイルロガー <br/>
    /// static readonly にすることでアプリケーション全体で共有
    /// </summary>
    protected static readonly Utilities.CustomFileLogger _responseLogger = new(
        filePath: "./logs/response.log",
        logLevel: Utilities.CustomFileLogger.LogLevel.INFO,
        maxFileSizeKB: 1024 // 1MB
    );

    /// <summary>
    /// テスト用のカスタムファイルロガー <br/>
    /// static readonly にすることでアプリケーション全体で共有
    /// </summary>
    protected static readonly Utilities.CustomFileLogger _testLogger = new(
        filePath: "./logs/test.log",
        logLevel: Utilities.CustomFileLogger.LogLevel.DEBUG,
        maxFileSizeKB: 1024 // 1MB
    );

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// アクション実行前に呼ばれるフィルタ <br/>
    /// - APIキー認証 <br/>
    /// - IPアドレスチェック <br/>
    /// - ストップウォッチ開始 <br/>
    /// </summary>
    /// <param name="context"></param>
    /// --------------------------------------------------------------------------------
    [Authorize(AuthenticationSchemes = "ApiKey")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuting(ActionExecutingContext context) {
        _logger.LogInformation("Action {ActionName} executing...", context.ActionDescriptor.DisplayName);
        // IPアドレスチェック（ローカルのみ許可）
        if (!IsValidIpAddress()) {
            context.Result = StatusCode(403, "許可されていないIPアドレスです (ローカルのみ許可)");
            return;
        }
        _logger.LogDebug("IP address validation OK.");
        // 実行時間計測用ストップウォッチ開始
        var sw = new Stopwatch();
        sw.Start();
        context.HttpContext.Items[StopwatchKey] = sw;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// アクション実行後に呼ばれるフィルタ <br/>
    /// - ストップウォッチを止める <br/>
    /// - 実行時間をログ出力する <br/>
    /// </summary>
    /// <param name="context">ActionExecutedContext</param>
    /// --------------------------------------------------------------------------------
    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuted(ActionExecutedContext context) {
        if (context.HttpContext.Items[StopwatchKey] is Stopwatch sw) {
            sw.Stop();
            var msec = sw.ElapsedMilliseconds;
            var logObject = new {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                Path = context.HttpContext.Request.Path,
                Method = context.HttpContext.Request.Method,
                Action = context.ActionDescriptor.DisplayName,
                StatusCode = context.HttpContext.Response.StatusCode,
                ElapsedMilliseconds = msec,
                RemoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.HttpContext.Request.Headers.UserAgent.ToString()
            };
            string json = JsonSerializer.Serialize(logObject);
            _responseLogger.WriteLine(json);
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// リモートIPアドレスが "localhost" かどうかを判定する
    /// </summary>
    /// <returns>true: OK / false: NG</returns>
    /// --------------------------------------------------------------------------------
    private bool IsValidIpAddress() {
        var remoteIp = HttpContext.Connection.RemoteIpAddress;
        // リモートIPが127.0.0.1または::1かどうか
        if (!(remoteIp?.ToString() == "127.0.0.1" || remoteIp?.ToString() == "::1")) {
            return false;
        }
        return true;
    }

}