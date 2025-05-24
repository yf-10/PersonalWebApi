using PersonalWebApi.Models.Data;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// データベースの行データをエンティティに変換するマッパークラス
/// </summary>
/// --------------------------------------------------------------------------------
public class SalaryMapper : IEntityMapper<Salary> {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 行データ（辞書）をエンティティに変換する
    /// </summary>
    /// <param name="row">データベースから取得した1行分の辞書</param>
    /// <returns>エンティティ</returns>
    /// <exception cref="InvalidCastException">必須フィールドがnullの場合にスロー</exception>
    /// --------------------------------------------------------------------------------
    public Salary MapRowToEntity(Dictionary<string, object?> row) {
        return new Salary(
            row["month"] as string ?? throw new InvalidCastException("month is null"),
            row["deduction"] is bool b ? b : Convert.ToBoolean(row["deduction"]),
            row["payment_item"] as string ?? throw new InvalidCastException("payment_item is null"),
            new Money(
                row["amount"] is decimal d ? d : Convert.ToDecimal(row["amount"]),
                row["currency_code"] as string ?? throw new InvalidCastException("currency_code is null")
            ),
            row["created_by"] as string ?? "system",
            row["updated_by"] as string ?? "system",
            row["created_at"] is DateTime dt ? dt : Convert.ToDateTime(row["created_at"]),
            row["updated_at"] is DateTime ut ? ut : Convert.ToDateTime(row["updated_at"]),
            row["exclusive_flag"] is int ef ? ef : Convert.ToInt32(row["exclusive_flag"])
        );
    }

}