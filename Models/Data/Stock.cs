namespace PersonalWebApi.Models.Data;
/// --------------------------------------------------------------------------------
/// <summary>
/// 保有株式データ
/// </summary>
/// --------------------------------------------------------------------------------
public class Stock {
    /// <summary>証券コード（主キー）</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>銘柄名</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>保有数量</summary>
    public decimal Quantity { get; set; }

    /// <summary>買付単価</summary>
    public decimal PurchasePrice { get; set; }

    /// <summary>買付日</summary>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>メモ</summary>
    public string? Memo { get; set; }

    /// <summary>登録日時</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>更新日時</summary>
    public DateTime UpdatedAt { get; set; }
}