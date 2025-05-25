using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// SQL文およびパラメータ生成ヘルパークラス
/// </summary>
/// --------------------------------------------------------------------------------
public class StockSqlHelper : ISqlHelper<Stock> {
    private const string TableName = "stock";
    private const string SelectColumns =
    """
        code,
        name,
        quantity,
        purchase_price,
        purchase_date,
        memo,
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
            code
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
            code = @code
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
            code,
            name,
            quantity,
            purchase_price,
            purchase_date,
            memo,
            created_at,
            updated_at
        )
        VALUES
        (
            @code,
            @name,
            @quantity,
            @purchase_price,
            @purchase_date,
            @memo,
            NOW(),
            NOW()
        )
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT or UPDATE] 主キー重複時は更新、なければ登録 for PostgreSQL
    /// </summary>
    /// <returns>SQL</returns>
    /// --------------------------------------------------------------------------------
    public string GetInsertOrUpdateSql() =>
        $"""
        INSERT INTO
            {TableName}
        (
            code,
            name,
            quantity,
            purchase_price,
            purchase_date,
            memo,
            created_at,
            updated_at
        )
        VALUES
        (
            @code,
            @name,
            @quantity,
            @purchase_price,
            @purchase_date,
            @memo,
            NOW(),
            NOW()
        )
        ON CONFLICT (code) DO UPDATE SET
            name = EXCLUDED.name,
            quantity = EXCLUDED.quantity,
            purchase_price = EXCLUDED.purchase_price,
            purchase_date = EXCLUDED.purchase_date,
            memo = EXCLUDED.memo,
            updated_at = NOW()
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
            name = @name,
            quantity = @quantity,
            purchase_price = @purchase_price,
            purchase_date = @purchase_date,
            memo = @memo,
            updated_at = NOW()
        WHERE
            code = @code
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [DELETE] 主キー指定で削除
    /// </summary>
    /// <returns>SQL</returns>
    /// --------------------------------------------------------------------------------
    public string GetDeleteSql() =>
        $"""
        DELETE FROM
            {TableName}
        WHERE
            code = @code
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT/UPDATE] SQLパラメータコレクションに変換する
    /// </summary>
    /// <param name="entity">BatchlogDetailエンティティ</param>
    /// <returns>パラメータコレクション</returns>
    /// --------------------------------------------------------------------------------
    public QueryParameterCollection ToParameterCollection(Stock entity) {
        return [
            new("@code", entity.Code),
            new("@name", entity.Name),
            new("@quantity", entity.Quantity),
            new("@purchase_price", entity.PurchasePrice),
            new("@purchase_date", entity.PurchaseDate),
            new("@memo", entity.Memo)
        ];
    }

}