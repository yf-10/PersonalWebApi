using System.Collections.Generic;
using PersonalWebApi.Models.Data;

namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// Mapper class for converting a database row to a Salary entity.
/// Implements IEntityMapper for Salary.
/// </summary>
public class SalaryMapper : IEntityMapper<Salary> {
    public Salary MapRowToObject(Dictionary<string, object?> row) {
        return new Salary(
            row["month"] as string ?? throw new InvalidCastException("month is null"),
            row["deduction"] is bool b ? b : Convert.ToBoolean(row["deduction"]),
            row["payment_item"] as string ?? throw new InvalidCastException("payment_item is null"),
            new Money(
                row["amount"] is decimal d ? d : Convert.ToDecimal(row["amount"]),
                row["currency_code"] as string ?? throw new InvalidCastException("currency_code is null")
            )
        );
    }
}