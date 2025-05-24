using PersonalWebApi.Models.Data;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// データベースの行データをエンティティに変換するマッパークラス
/// </summary>
/// --------------------------------------------------------------------------------
public class BatchlogDetailMapper : IEntityMapper<BatchlogDetail> {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 行データ（辞書）をエンティティに変換する
    /// </summary>
    /// <param name="row">データベースから取得した1行分の辞書</param>
    /// <returns>エンティティ</returns>
    /// <exception cref="InvalidCastException">必須フィールドがnullの場合にスロー</exception>
    /// --------------------------------------------------------------------------------
    public BatchlogDetail MapRowToEntity(Dictionary<string, object?> row) {
        return new BatchlogDetail(
            row["uuid"] as string ?? throw new InvalidCastException("uuid is null"),
            row["log_no"] is int i ? i : Convert.ToInt32(row["log_no"]),
            row["log_msg"] as string,
            row["log_time"] as DateTime? ?? throw new InvalidCastException("log_time is null"),
            row["created_by"] as string ?? throw new InvalidCastException("created_by is null"),
            row["updated_by"] as string ?? throw new InvalidCastException("updated_by is null"),
            row["created_at"] as DateTime?,
            row["updated_at"] as DateTime?
        );
    }

}