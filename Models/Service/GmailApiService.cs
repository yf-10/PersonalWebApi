using System.Text;

using Microsoft.Extensions.Options;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;

namespace PersonalWebApi.Models.Service;
/// --------------------------------------------------------------------------------
/// <summary>
/// サービスクラス
/// </summary>
/// --------------------------------------------------------------------------------
public class GmailApiService(ILogger logger, IOptions<AppSettings> options) : BaseService(logger, options) {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 指定ラベルの最新メールを取得し本文をパースして給与情報リストを返す
    /// </summary>
    /// <returns>給与情報リスト（該当メールがなければ空リスト）</returns>
    /// --------------------------------------------------------------------------------
    public async Task<List<Salary>> GetLatestSalaryFromGmailAsync() {
        // Google認証情報の取得（OAuth2）
        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets {
                ClientId = _options.Value.GmailApi.ClientId,
                ClientSecret = _options.Value.GmailApi.ClientSecret
            },
            [Google.Apis.Gmail.v1.GmailService.Scope.GmailReadonly],
            _options.Value.ApplicationName,
            CancellationToken.None,
            new FileDataStore("GmailApiTokenStore", true)
        );

        // Gmailサービス初期化
        var service = new Google.Apis.Gmail.v1.GmailService(new BaseClientService.Initializer {
            HttpClientInitializer = credential,
            ApplicationName = _options.Value.ApplicationName
        });

        // ラベル名からラベルIDを取得（カスタムラベルはID指定が必要）
        var labelName = _options.Value.GmailApi.TargetLabelName ?? "INBOX";
        var labelListReq = service.Users.Labels.List("me");
        var labelListRes = await labelListReq.ExecuteAsync();
        var labelId = labelListRes.Labels.FirstOrDefault(l => l.Name == labelName)?.Id;
        if (string.IsNullOrEmpty(labelId)) return [];

        // 指定ラベルのメールを取得
        var request = service.Users.Messages.List("me");
        request.MaxResults = _options.Value.GmailApi.MaxResults; // 最大取得件数
        request.LabelIds = new List<string> { labelId };
        var messagesResponse = await request.ExecuteAsync();

        if (messagesResponse.Messages == null || messagesResponse.Messages.Count == 0) return [];
        _logger.LogInformation("{msg}", $"Found {messagesResponse.Messages.Count} messages with label '{labelName}'.");

        // 取得した全メールを解析してSalaryリストに追加
        var allSalaries = new List<Salary>();
        foreach (var msg in messagesResponse.Messages) {
            var messageRequest = service.Users.Messages.Get("me", msg.Id);
            var message = await messageRequest.ExecuteAsync();
            string body = GetBodyFromMessage(message);

            // 件名を取得
            string subject = message.Payload?.Headers?.FirstOrDefault(h => h.Name == "Subject")?.Value ?? string.Empty;

            // 件名をParseに渡す
            var salaries = SalaryMailParser.Parse(body, subject, _options.Value.ApplicationName, _options.Value.ApplicationName);
            if (salaries != null && salaries.Count > 0)
                allSalaries.AddRange(salaries);
        }
        return allSalaries;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// Gmail APIのMessageオブジェクトから本文テキストを抽出する
    /// </summary>
    /// <param name="message">Gmail APIのMessageオブジェクト</param>
    /// <returns>本文テキスト（なければ空文字）</returns>
    /// --------------------------------------------------------------------------------
    private static string GetBodyFromMessage(Message message) {
        // プレーンテキストパート（text/plain）があれば優先して取得
        if (message.Payload?.Parts != null) {
            foreach (var part in message.Payload.Parts) {
                if (part.MimeType == "text/plain" && part.Body?.Data != null) {
                    return DecodeBase64Url(part.Body.Data);
                }
            }
        }
        // プレーンテキストパートがなければ、Payload.Body.Dataを利用
        if (message.Payload?.Body?.Data != null) {
            return DecodeBase64Url(message.Payload.Body.Data);
        }
        return string.Empty;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// Gmail APIのBase64Urlエンコードされた文字列をデコードしてUTF-8文字列に変換する
    /// </summary>
    /// <param name="base64Url">Base64Urlエンコード文字列</param>
    /// <returns>デコード後の文字列</returns>
    /// --------------------------------------------------------------------------------
    private static string DecodeBase64Url(string base64Url) {
        // Gmail APIのBase64Urlは「-」「_」を「+」「/」に変換し、パディングを補う必要がある
        string fixedBase64 = base64Url.Replace('-', '+').Replace('_', '/');
        switch (fixedBase64.Length % 4) {
            case 2: fixedBase64 += "=="; break;
            case 3: fixedBase64 += "="; break;
        }
        var bytes = Convert.FromBase64String(fixedBase64);
        return Encoding.UTF8.GetString(bytes);
    }

}