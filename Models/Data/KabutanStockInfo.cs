namespace PersonalWebApi.Models.Data;

/// --------------------------------------------------------------------------------
/// <summary>
/// Kabutanの株情報を表すクラス <br/>
/// https://info.kabutan.jp/
/// </summary>
/// --------------------------------------------------------------------------------
public class KabutanStockInfo {
    public string? Code { get; set; }
    public string? CompanyName { get; set; }
    public string? Price { get; set; }
    public string? Diff { get; set; }
    public string? DiffPercent { get; set; }
    public string? Open { get; set; }
    public string? High { get; set; }
    public string? Low { get; set; }
    public string? Close { get; set; }
    public string? Volume { get; set; }
    public string? TradingValue { get; set; }
    public string? PER { get; set; }
    public string? PBR { get; set; }
    public string? DividendYield { get; set; }
    public string? SharesOutstanding { get; set; }
    public string? Unit { get; set; }
    public string? Industry { get; set; }
}