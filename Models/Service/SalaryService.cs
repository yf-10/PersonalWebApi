using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.DataAccess;
using PersonalWebApi.Utilities;
using System.Globalization;
using System.Transactions;

namespace PersonalWebApi.Models.Service;

/// <summary>
/// Service class for Salary data registration with transaction management.
/// </summary>
public class SalaryService
{
    private readonly ILogger<SalaryService> _logger = new Logger<SalaryService>(new LoggerFactory());
    private readonly IOptions<AppSettings> _options;

    public SalaryService(IOptions<AppSettings> options)
    {
        _options = options;
    }

    /// <summary>
    /// Registers multiple Salary records in a transaction using TransactionScope.
    /// </summary>
    /// <param name="salaries">The list of Salary objects to register.</param>
    /// <returns>The number of records inserted.</returns>
    public int RegisterSalariesWithTransaction(List<Salary> salaries)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var worker = new PostgresDbWorker(_options);
        var repository = new SalaryRepository(worker);
        int count = repository.InsertAll(salaries);
        scope.Complete();
        return count;
    }

    /// <summary>
    /// Retrieves all Salary records from the database.
    /// </summary>
    /// <returns>List of Salary objects.</returns>
    public List<Salary> GetAllSalaries()
    {
        var worker = new PostgresDbWorker(_options);
        var repository = new SalaryRepository(worker);
        return repository.GetAll();
    }

    /// <summary>
    /// Retrieves Salary records filtered by a specified period.
    /// </summary>
    /// <param name="startYm">Start year-month in "yyyyMM" format.</param>
    /// <param name="endYm">End year-month in "yyyyMM" format.</param>
    /// <returns>List of filtered Salary objects.</returns>
    public List<Salary> GetSalariesByPeriod(string? startYm, string? endYm)
    {
        // Retrieve all Salary records
        var allSalaries = GetAllSalaries();

        // Parse filter conditions
        DateTime? start = null;
        DateTime? end = null;
        if (!string.IsNullOrEmpty(startYm) && DateTime.TryParseExact(startYm, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var s))
            start = s;
        if (!string.IsNullOrEmpty(endYm) && DateTime.TryParseExact(endYm, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var e))
            end = e;

        // Filter based on Month property of Salary
        var filtered = allSalaries.Where(sal =>
        {
            if (DateTime.TryParseExact(sal.Month, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ym))
            {
                bool afterStart = !start.HasValue || ym >= start.Value;
                bool beforeEnd = !end.HasValue || ym <= end.Value;
                return afterStart && beforeEnd;
            }
            return false;
        }).ToList();

        return filtered;
    }
}