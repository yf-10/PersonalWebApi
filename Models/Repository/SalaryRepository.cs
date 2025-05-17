using System.Collections.Generic;
using System.Threading.Tasks;

using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.Repository;

/// <summary>
/// Repository for accessing Salary data in the database.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SalaryRepository"/> class.
/// </remarks>
/// <param name="db">The PostgresqlWorker instance for database operations.</param>
public class SalaryRepository(PostgresqlWorker db) {
    private readonly PostgresqlWorker _db = db;

    /// <summary>
    /// Gets all salary records from the database.
    /// </summary>
    /// <returns>A list of Salary objects.</returns>
    public List<Salary> GetAll()
    {
        var sql = "SELECT month, deduction, payment_item, money_value, money_currency FROM salary";
        var rows = _db.ExecuteSqlGetList(sql);
        var result = new List<Salary>();
        foreach (var row in rows)
        {
            result.Add(MapRowToSalary(row));
        }
        return result;
    }

    /// <summary>
    /// Gets all salary records from the database asynchronously.
    /// </summary>
    /// <returns>A list of Salary objects.</returns>
    public async Task<List<Salary>> GetAllAsync()
    {
        var sql = "SELECT month, deduction, payment_item, money_value, money_currency FROM salary";
        var rows = await _db.ExecuteSqlGetListAsync(sql);
        var result = new List<Salary>();
        foreach (var row in rows)
        {
            result.Add(MapRowToSalary(row));
        }
        return result;
    }

    /// <summary>
    /// Inserts a new salary record into the database.
    /// </summary>
    /// <param name="salary">The Salary object to insert.</param>
    /// <returns>The number of rows affected.</returns>
    public int Insert(Salary salary)
    {
        var sql = "INSERT INTO salary (month, deduction, payment_item, money_value, money_currency) VALUES (@month, @deduction, @payment_item, @money_value, @money_currency)";
        var prms = new List<SqlParameter>
        {
            new("@month", salary.Month),
            new("@deduction", salary.Deduction),
            new("@payment_item", salary.PaymentItem),
            new("@money_value", salary.Money.Amount),
            new("@money_currency", salary.Money.CurrencyCode)
        };
        return _db.ExecuteSql(sql, prms);
    }

    /// <summary>
    /// Inserts a new salary record into the database asynchronously.
    /// </summary>
    /// <param name="salary">The Salary object to insert.</param>
    /// <returns>The number of rows affected.</returns>
    public Task<int> InsertAsync(Salary salary)
    {
        var sql = "INSERT INTO salary (month, deduction, payment_item, money_value, money_currency) VALUES (@month, @deduction, @payment_item, @money_value, @money_currency)";
        var prms = new List<SqlParameter>
        {
            new("@month", salary.Month),
            new("@deduction", salary.Deduction),
            new("@payment_item", salary.PaymentItem),
            new("@money_value", salary.Money.Amount),
            new("@money_currency", salary.Money.CurrencyCode)
        };
        return _db.ExecuteSqlAsync(sql, prms);
    }

    /// <summary>
    /// Maps a dictionary row to a Salary object.
    /// </summary>
    private static Salary MapRowToSalary(Dictionary<string, object> row)
    {
        var money = new Money(
            (int)row["money_value"],
            (string)row["money_currency"]
        );
        return new Salary(
            (string)row["month"],
            Convert.ToBoolean(row["deduction"]),
            (string)row["payment_item"],
            money
        );
    }
}