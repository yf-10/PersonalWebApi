using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.DataAccess;
using PersonalWebApi.Utilities;
using System.Transactions;

namespace PersonalWebApi.Models.Service;

/// <summary>
/// Service class for BatchlogMain data registration, update, and retrieval with transaction management.
/// </summary>
public class BatchlogService(IConfiguration configuration) {
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Registers a BatchlogMain record in a transaction.
    /// </summary>
    /// <param name="batchlog">The BatchlogMain object to register.</param>
    /// <returns>The number of records inserted.</returns>
    public int RegisterBatchlogWithTransaction(BatchlogMain batchlog) {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var worker = new PostgresDbWorker(_configuration);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        int count = repository.Insert(batchlog);
        scope.Complete();
        return count;
    }

    /// <summary>
    /// Registers a BatchlogDetail record in a transaction.
    /// </summary>
    /// <param name="detail">The BatchlogDetail object to register.</param>
    /// <returns>The number of records inserted.</returns>
    public int RegisterBatchlogDetailWithTransaction(BatchlogDetail detail) {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var worker = new PostgresDbWorker(_configuration);
        var repository = new BatchlogDetailRepository(worker, new BatchlogDetailSqlHelper(), new BatchlogDetailMapper());
        int count = repository.Insert(detail);
        scope.Complete();
        return count;
    }

    /// <summary>
    /// Updates a BatchlogMain record in a transaction.
    /// </summary>
    /// <param name="batchlog">The BatchlogMain object to update.</param>
    /// <returns>The number of records updated.</returns>
    public int UpdateBatchlogWithTransaction(BatchlogMain batchlog) {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var worker = new PostgresDbWorker(_configuration);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        int count = repository.Update(batchlog);
        scope.Complete();
        return count;
    }

    /// <summary>
    /// Retrieves all BatchlogMain records from the database.
    /// </summary>
    /// <returns>List of BatchlogMain objects.</returns>
    public List<BatchlogMain> GetAllBatchlogs() {
        var worker = new PostgresDbWorker(_configuration);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        return repository.GetAll();
    }
}