using PersonalWebApi.Models.Data;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// データベースの行データをエンティティに変換するマッパークラス
/// </summary>
/// --------------------------------------------------------------------------------
public class BatchlogMainMapper : IEntityMapper<BatchlogMain> {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 行データ（辞書）をエンティティに変換する
    /// </summary>
    /// <param name="row">データベースから取得した1行分の辞書</param>
    /// <returns>エンティティ</returns>
    /// <exception cref="InvalidCastException">必須フィールドがnullの場合にスロー</exception>
    /// --------------------------------------------------------------------------------
    public BatchlogMain MapRowToEntity(Dictionary<string, object?> row) {
        return new BatchlogMain(
            row["uuid"] as string ?? throw new InvalidCastException("uuid is null"),
            Enum.TryParse<BatchlogStatus>(row["status"] as string ?? throw new InvalidCastException("status is null"), out var status) ? status : throw new InvalidCastException("Invalid status value"),
            row["program_id"] as string ?? throw new InvalidCastException("program_id is null"),
            row["program_name"] as string,
            row["start_time"] as DateTime?,
            row["end_time"] as DateTime?,
            row["created_by"] as string ?? throw new InvalidCastException("created_by is null"),
            row["updated_by"] as string ?? throw new InvalidCastException("updated_by is null"),
            row["created_at"] as DateTime?,
            row["updated_at"] as DateTime?
        );
    }

}