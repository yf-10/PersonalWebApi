using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using PersonalWebApi.Models.Config;

namespace PersonalWebApi.Utilities;
/// --------------------------------------------------------------------------------
/// <summary>
/// APIキー認証ハンドラ
/// </summary>
/// --------------------------------------------------------------------------------
public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    IOptionsSnapshot<AppSettings> appSettings,
    ILoggerFactory logger,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder) {
    private readonly IOptionsSnapshot<AppSettings> _appSettings = appSettings;
    private readonly ILogger<ApiKeyAuthenticationHandler> _logger = logger.CreateLogger<ApiKeyAuthenticationHandler>();
    private readonly string ApiKeyHeaderName = appSettings.Value.ApiSettings.ApiKeyHeaderName;
    private readonly string ApiKey = appSettings.Value.ApiSettings.ApiKey;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// APIキー認証の実装
    /// </summary>
    /// <returns>認証結果</returns>
    /// --------------------------------------------------------------------------------
    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        // テストモードの場合はAPIキー認証をスキップ
        if (_appSettings.Value.IsTest) {
            _logger.LogWarning("API Key authentication is disabled in test mode.");
            return Task.FromResult(AuthenticateResult.Success(CreateAuthenticationTicket("TestUser")));
        }
        // ヘッダーからAPIキーを取得
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues)) {
            return Task.FromResult(AuthenticateResult.Fail("API Key was not provided."));
        }
        // APIキーの値と比較し正しいか検証
        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(providedApiKey) || !string.Equals(providedApiKey, ApiKey, StringComparison.Ordinal)) {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key."));
        }
        // 認証成功
        return Task.FromResult(AuthenticateResult.Success(CreateAuthenticationTicket("ApiKeyUser")));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 認証チケットを作成するヘルパーメソッド
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    private AuthenticationTicket CreateAuthenticationTicket(string userName) {
        var claims = new[] { new Claim(ClaimTypes.Name, userName) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        return new AuthenticationTicket(principal, Scheme.Name);
    }

}