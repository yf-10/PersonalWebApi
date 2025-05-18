using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// SQL and parameter generation helper for SalaryRepository.
/// Implements ISqlHelper for Salary entity.
/// </summary>
public class SalarySqlHelper : ISqlHelper<Salary> {
    private const string TableName = "salary";
    private const string SelectColumns = @"
        month,
        deduction,
        payment_item,
        amount,
        currency_code";

    public string GetSelectAllSql() =>
        $@"SELECT {SelectColumns}
            FROM {TableName}
            ORDER BY month, payment_item";

    public string GetSelectByIdSql() =>
        $@"SELECT {SelectColumns}
            FROM {TableName}
            WHERE month = @month AND payment_item = @payment_item";

    public string GetInsertSql() =>
        $@"INSERT INTO {TableName} (
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
            ) VALUES (
                @month,
                @deduction,
                @payment_item,
                @amount,
                @currency_code,
                'test',
                'test',
                NOW(),
                NOW(),
                0
            )";

    public string GetUpdateSql() =>
        $@"UPDATE {TableName} SET
                deduction = @deduction,
                amount = @amount,
                currency_code = @currency_code
            WHERE
                month = @month AND payment_item = @payment_item";

    public QueryParameterCollection ToParameterCollection(Salary entity) {
        return [
            new("@month", entity.Month),
            new("@deduction", entity.Deduction),
            new("@payment_item", entity.PaymentItem),
            new("@amount", entity.Money.Amount),
            new("@currency_code", entity.Money.CurrencyCode)
        ];
    }

    public QueryParameterCollection ToIdParameterCollection(object id) {
        // id should be a tuple (string month, string paymentItem)
        if (id is not ValueTuple<string, string> key)
            throw new ArgumentException("id must be a tuple (string month, string paymentItem)");
        return [
            new("@month", key.Item1),
            new("@payment_item", key.Item2)
        ];
    }
}