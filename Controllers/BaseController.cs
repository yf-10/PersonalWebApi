using System.Diagnostics;

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
[Authorize(AuthenticationSchemes = "ApiKey")]
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

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// アクション実行前に呼ばれるフィルタ <br/>
    /// - APIキー認証 <br/>
    /// - IPアドレスチェック <br/>
    /// - ストップウォッチ開始 <br/>
    /// </summary>
    /// <param name="context"></param>
    /// --------------------------------------------------------------------------------
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
            _logger.LogInformation("Action executed: {ElapsedMilliseconds} ms", msec);
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