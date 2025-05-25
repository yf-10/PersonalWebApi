using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// SQL文およびパラメータ生成ヘルパークラス
/// </summary>
/// --------------------------------------------------------------------------------
public class BatchlogDetailSqlHelper : ISqlHelper<BatchlogDetail> {
    private const string TableName = "batchlog_detail";
    private const string SelectColumns =
    """
        uuid,
        log_no,
        log_msg,
        log_time,
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
            uuid ASC, log_no ASC
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
            AND log_no = @log_no
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] バッチ実行識別子指定で取得
    /// </summary>
    /// <returns>SQL</returns>
    /// --------------------------------------------------------------------------------
    public string GetSelectByUuidSql() =>
        $"""
        SELECT
        {SelectColumns}
        FROM
            {TableName}
        WHERE
            uuid = @uuid
        ORDER BY
            log_no ASC
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] バッチ実行識別子指定で最新のログを取得
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public string GetLatestSql() =>
        $"""
        SELECT
        {SelectColumns}
        FROM
            {TableName}
        WHERE
            uuid = @uuid
            AND log_no = (
                SELECT
                    MAX(log_no)
                FROM
                    {TableName}
                WHERE
                    uuid = @uuid
            )
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
            log_no,
            log_msg,
            log_time,
            created_by,
            updated_by,
            created_at,
            updated_at
        )
        VALUES
        (
            @uuid,
            @log_no,
            @log_msg,
            @log_time,
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
            log_msg = @log_msg,
            log_time = @log_time,
            updated_by = @updated_by,
            updated_at = NOW()
        WHERE
            uuid = @uuid
            AND log_no = @log_no
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT/UPDATE] SQLパラメータコレクションに変換する
    /// </summary>
    /// <param name="entity">BatchlogDetailエンティティ</param>
    /// <returns>パラメータコレクション</returns>
    /// --------------------------------------------------------------------------------
    public QueryParameterCollection ToParameterCollection(BatchlogDetail entity) {
        return [
            new("@uuid", entity.Uuid),
            new("@log_no", entity.LogNo),
            new("@log_msg", entity.LogMsg),
            new("@log_time", entity.LogTime),
            new("@created_by", entity.CreatedBy ?? "unknown"),
            new("@updated_by", entity.UpdatedBy ?? "unknown")
        ];
    }

}