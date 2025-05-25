using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// SQL文およびパラメータ生成ヘルパラス
/// </summary>
/// --------------------------------------------------------------------------------
public class SalarySqlHelper : ISqlHelper<Salary> {
    private const string TableName = "salary";
    private const string SelectColumns =
    """
        month,
        deduction,
        payment_item,
        amount,
        currency_code,
        created_by,
        updated_by,
        created_at,
        updated_at,
        exclusive_flag
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
            month desc,
            deduction asc,
            payment_item asc
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
            month = @month
            AND deduction = @deduction
            AND payment_item = @payment_item
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 期間指定で取得（YYYYMM～YYYYMM）
    /// </summary>
    /// <returns>SQL</returns>
    /// --------------------------------------------------------------------------------
    public string GetSelectByMonthBetweenSql() =>
        $"""
        SELECT
        {SelectColumns}
        FROM
            {TableName}
        WHERE
            month BETWEEN @start_month AND @end_month
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
            month,
            deduction,
            payment_item,
            amount,
            currency_code,
            created_by,
            updated_by,
            created_at,
            updated_at,
            exclusive_flag
        )
        VALUES
        (
            @month,
            @deduction,
            @payment_item,
            @amount,
            @currency_code,
            @created_by,
            @updated_by,
            NOW(),
            NOW(),
            0
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
            amount = @amount,
            currency_code = @currency_code,
            updated_by = @updated_by,
            updated_at = NOW(),
            exclusive_flag = exclusive_flag + 1
        WHERE
            month = @month
            AND deduction = @deduction
            AND payment_item = @payment_item
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
            month,
            deduction,
            payment_item,
            amount,
            currency_code,
            created_by,
            updated_by,
            created_at,
            updated_at,
            exclusive_flag
        )
        VALUES
        (
            @month,
            @deduction,
            @payment_item,
            @amount,
            @currency_code,
            @created_by,
            @updated_by,
            NOW(),
            NOW(),
            0
        )
        ON CONFLICT (month, deduction, payment_item) DO UPDATE SET
            amount = EXCLUDED.amount,
            currency_code = EXCLUDED.currency_code,
            updated_by = EXCLUDED.updated_by,
            updated_at = NOW(),
            exclusive_flag = {TableName}.exclusive_flag + 1
        """;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT/UPDATE] SQLパラメータコレクションに変換する
    /// </summary>
    /// <param name="entity">BatchlogDetailエンティティ</param>
    /// <returns>パラメータコレクション</returns>
    /// --------------------------------------------------------------------------------
    public QueryParameterCollection ToParameterCollection(Salary entity) {
        return [
            new("@month", entity.Month),
            new("@deduction", entity.Deduction),
            new("@payment_item", entity.PaymentItem),
            new("@amount", entity.Money.Amount),
            new("@currency_code", entity.Money.CurrencyCode),
            new("@created_by", entity.CreatedBy),
            new("@updated_by", entity.UpdatedBy)
        ];
    }

}