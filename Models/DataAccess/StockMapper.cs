using PersonalWebApi.Models.Data;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// データベースの行データをエンティティに変換するマッパークラス
/// </summary>
/// --------------------------------------------------------------------------------
public class StockMapper : IEntityMapper<Stock> {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 行データ（辞書）をエンティティに変換する
    /// </summary>
    /// <param name="row">データベースから取得した1行分の辞書</param>
    /// <returns>エンティティ</returns>
    /// <exception cref="InvalidCastException">必須フィールドがnullの場合にスロー</exception>
    /// --------------------------------------------------------------------------------
    public Stock MapRowToEntity(Dictionary<string, object?> row) {
        return new Stock {
            Code = row["code"] as string ?? throw new InvalidCastException("code is null"),
            Name = row["name"] as string ?? throw new InvalidCastException("name is null"),
            Quantity = row["quantity"] is decimal d ? d : Convert.ToDecimal(row["quantity"]),
            PurchasePrice = row["purchase_price"] is decimal p ? p : Convert.ToDecimal(row["purchase_price"]),
            PurchaseDate = row["purchase_date"] is DateTime dt ? dt : row["purchase_date"] == null ? null : Convert.ToDateTime(row["purchase_date"]),
            Memo = row["memo"] as string,
            CreatedAt = row["created_at"] is DateTime cdt ? cdt : Convert.ToDateTime(row["created_at"]),
            UpdatedAt = row["updated_at"] is DateTime udt ? udt : Convert.ToDateTime(row["updated_at"])
        };
    }

}