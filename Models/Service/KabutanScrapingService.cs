using System.Text.RegularExpressions;

using Microsoft.Extensions.Options;

using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.Config;

namespace PersonalWebApi.Models.Service;
/// --------------------------------------------------------------------------------
/// <summary>
/// サービスクラス
/// </summary>
/// --------------------------------------------------------------------------------
public class KabutanScrapingService(ILogger logger, IOptions<AppSettings> options, IHttpClientFactory httpClientFactory) : BaseService(logger, options) {
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 株探から株価・指標情報を取得しパースする
    /// </summary>
    /// --------------------------------------------------------------------------------
    public async Task<KabutanStockInfo?> GetStockInfoAsync(string code) {
        var url = $"https://kabutan.jp/stock/?code={code}";
        var html = await _httpClientFactory.CreateClient().GetStringAsync(url);
        if (string.IsNullOrEmpty(html)) return null;

        string? Extract(string pattern, int matchIndex = 1) {
            var matches = Regex.Matches(html, pattern, RegexOptions.Singleline);
            return matches.Count >= matchIndex
                ? matches[matchIndex - 1].Groups[1].Value.Trim()
                : null;
        }

        return new KabutanStockInfo {
            Code = code,
            CompanyName = Extract(@"<h3[^>]*class=""jp""[^>]*>([^<\r\n]+)</h3>") ?? Extract(@"<h3[^>]*>([^<\r\n]+)</h3>"),
            Price = Extract(@"<span class=""kabuka"">([\d,\.]+)円</span>"),
            Diff = Extract(@"<dt>前日比</dt>\s*<dd><span.*?>([+\-\d\.]+)</span></dd>"),
            DiffPercent = Extract(@"<dt>前日比</dt>\s*<dd><span.*?>[+\-\d\.]+</span></dd>\s*<dd><span.*?>([+\-\d\.]+)</span>%</dd>"),
            Open = Extract(@"<th[^>]*>始値</th>\s*<td>([\d,\.]+)</td>"),
            High = Extract(@"<th[^>]*>高値</th>\s*<td>([\d,\.]+)</td>"),
            Low = Extract(@"<th[^>]*>安値</th>\s*<td>([\d,\.]+)</td>"),
            Close = Extract(@"<th[^>]*>終値</th>\s*<td>([\d,\.]+)</td>"),
            Volume = Extract(@"<th[^>]*>出来高</th>\s*<td>([\d,]+)[^<]*株</td>"),
            TradingValue = Extract(@"<th[^>]*>売買代金</th>\s*<td>([\d,]+)[^<]*百万円</td>"),
            PER = Extract(@"<td>([\d\.]+)<span[^>]*>倍</span></td>"),
            PBR = Extract(@"<td>([\d\.]+)<span[^>]*>倍</span></td>", 2),
            DividendYield = Extract(@"<td>([\d\.]+)<span[^>]*>％</span></td>"),
            SharesOutstanding = Extract(@"<th[^>]*>発行済株式数</th>\s*<td>([\d,]+)[^<]*株</td>"),
            Unit = Extract(@"<th[^>]*>単元株数</th>\s*<td>([\d,]+)[^<]*株</td>"),
            Industry = Extract(@"<th[^>]*>業種</th>\s*<td>([^<]+)</td>")
        };
    }

}