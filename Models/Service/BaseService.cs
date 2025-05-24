using Microsoft.Extensions.Options;

using PersonalWebApi.Models.Config;

namespace PersonalWebApi.Models.Service;
/// --------------------------------------------------------------------------------
/// <summary>
/// サービスクラスの基底クラス
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
/// --------------------------------------------------------------------------------
public abstract class BaseService(ILogger logger, IOptions<AppSettings> options) {

    /// <summary>
    /// 標準ロガー
    /// </summary>
    protected readonly ILogger _logger = logger;

    /// <summary>
    /// アプリケーション設定オプション
    /// </summary>
    protected readonly IOptions<AppSettings> _options = options;

}