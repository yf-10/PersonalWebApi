using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.DataAccess;
using PersonalWebApi.Utilities;
using System.Transactions;

namespace PersonalWebApi.Models.Service;

/// <summary>
/// Service class for Salary data registration with transaction management.
/// </summary>
public class SalaryService(IConfiguration configuration) {
    private readonly ILogger<SalaryService> _logger = new Logger<SalaryService>(new LoggerFactory());
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Registers multiple Salary records in a transaction using TransactionScope.
    /// </summary>
    /// <param name="salaries">The list of Salary objects to register.</param>
    /// <returns>The number of records inserted.</returns>
    public int RegisterSalariesWithTransaction(List<Salary> salaries) {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var worker = new PostgresDbWorker(_configuration);
        var repository = new SalaryRepository(worker);
        int count = repository.InsertAll(salaries);
        scope.Complete();
        return count;
    }

    /// <summary>
    /// Retrieves all Salary records from the database.
    /// </summary>
    /// <returns>List of Salary objects.</returns>
    public List<Salary> GetAllSalaries() {
        var worker = new PostgresDbWorker(_configuration);
        var repository = new SalaryRepository(worker);
        return repository.GetAll();
    }
}