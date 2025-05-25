using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// SQL文およびパラメータ生成ヘルパークラス
/// </summary>
/// --------------------------------------------------------------------------------
public class BatchlogMainSqlHelper : ISqlHelper<BatchlogMain> {
    private const string TableName = "batchlog_main";
    private const string SelectColumns =
    """
        uuid,
        status,
        program_id,
        program_name,
        start_time,
        end_time,
        created_by,
        updated_by,
        created_at,
        updated_at
    """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 全件取得
    /// </summary>
    /// <returns>SQL</returns>
    /// --------------------------------------------------------------------------------
    public string GetSelectSql() =>
        $"""
        SELECT
        {SelectColumns}
        FROM
            {TableName}
        ORDER BY
            uuid asc
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 主キー指定で取得
    /// </summary>
    /// <returns>SQL</returns>
    /// --------------------------------------------------------------------------------
    public string GetSelectByIdSql() =>
        $"""
        SELECT
        {SelectColumns}
        FROM
            {TableName}
        WHERE
            uuid = @uuid
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録
    /// </summary>
    /// <returns>SQL</returns>
    /// --------------------------------------------------------------------------------
    public string GetInsertSql() =>
        $"""
        INSERT INTO
            {TableName}
        (
            uuid,
            status,
            program_id,
            program_name,
            start_time,
            end_time,
            created_by,
            updated_by,
            created_at,
            updated_at
        )
        VALUES
        (
            @uuid,
            @status,
            @program_id,
            @program_name,
            @start_time,
            @end_time,
            @created_by,
            @updated_by,
            NOW(),
            NOW()
        )
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [UPDATE] 既存レコード更新
    /// </summary>
    /// <returns>SQL</returns>
    /// --------------------------------------------------------------------------------
    public string GetUpdateSql() =>
        $"""
        UPDATE
            {TableName}
        SET
            status = @status,
            program_id = @program_id,
            program_name = @program_name,
            start_time = @start_time,
            end_time = @end_time,
            updated_by = @updated_by,
            updated_at = NOW()
        WHERE
            uuid = @uuid
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT/UPDATE] SQLパラメータコレクションに変換する
    /// </summary>
    /// <param name="entity">BatchlogDetailエンティティ</param>
    /// <returns>パラメータコレクション</returns>
    /// --------------------------------------------------------------------------------
    public QueryParameterCollection ToParameterCollection(BatchlogMain log) {
        return [
            new("@uuid", log.Uuid),
            new("@status", log.Status.ToString()),
            new("@program_id", log.ProgramId),
            new("@program_name", log.ProgramName),
            new("@start_time", log.StartTime),
            new("@end_time", log.EndTime),
            new("@created_by", log.CreatedBy),
            new("@updated_by", log.UpdatedBy)
        ];
    }

}