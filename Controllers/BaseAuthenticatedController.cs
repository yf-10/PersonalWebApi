using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using PersonalWebApi.Models.Config;
using System.Diagnostics;

namespace PersonalWebApi.Controllers;

/// <summary>
/// Base controller for authentication
/// </summary>
public abstract class BaseAuthenticatedController : ControllerBase, IActionFilter
{
    protected readonly ILogger logger;
    protected readonly IOptions<AppSettings> options;

    /// <summary>
    /// Default API key for authentication
    /// </summary>
    private const string DefaultApiKey = "e74c2f8a-3d92-4a67-95e2-8c874baf37db";

    /// <summary>
    /// Stopwatch key for HttpContext.Items
    /// </summary>
    private const string StopwatchKey = "__BaseAuthenticatedController_Stopwatch";

    /// <summary>
    /// コンストラクタ（POCOクラス＋IOptionsパターン）
    /// </summary>
    /// <param name="logger">ILoggerインスタンス</param>
    /// <param name="options">IOptionsでバインドされたAppSettings</param>
    protected BaseAuthenticatedController(ILogger logger, IOptions<AppSettings> options)
    {
        this.logger = logger;
        this.options = options;
    }

    /// <summary>
    /// Called before the action executes. Performs API key authentication and IP address validation.
    /// Starts a stopwatch to measure execution time.
    /// </summary>
    /// <param name="context">ActionExecutingContext</param>
    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        logger.LogInformation("Starting action execution.");
        // Check if the API key is valid
        if (!IsAuthenticated(context))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        logger.LogDebug("API key is valid.");
        // Check if the IP address is valid (only allow local requests)
        if (!IsValidIpAddress())
        {
            context.Result = StatusCode(403, "Forbidden: Only local requests are allowed.");
            return;
        }
        logger.LogDebug("IP address is valid.");
        // Start stopwatch for measuring action execution time
        var sw = new Stopwatch();
        sw.Start();
        context.HttpContext.Items[StopwatchKey] = sw;
    }

    /// <summary>
    /// Checks if the request is authenticated by validating the API key in the header.
    /// </summary>
    /// <param name="context">ActionExecutingContext</param>
    /// <returns>True if authenticated, otherwise false.</returns>
    private static bool IsAuthenticated(ActionExecutingContext context)
    {
        var apiKey = context.HttpContext.Request.Headers["X-Api-Key"].FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey) || apiKey != DefaultApiKey)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the remote IP address is localhost (127.0.0.1 or ::1).
    /// </summary>
    /// <returns>True if the request is from localhost, otherwise false.</returns>
    private bool IsValidIpAddress()
    {
        var remoteIp = HttpContext.Connection.RemoteIpAddress;
        // Check if the remote IP address is localhost
        if (!(remoteIp?.ToString() == "127.0.0.1" || remoteIp?.ToString() == "::1"))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Called after the action executes.
    /// Stops the stopwatch and logs the elapsed time.
    /// </summary>
    /// <param name="context">ActionExecutedContext</param>
    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Items[StopwatchKey] is Stopwatch sw)
        {
            sw.Stop();
            var msec = sw.ElapsedMilliseconds;
            logger.LogInformation("Action executed in {ElapsedMilliseconds} ms", msec);
        }
    }
}