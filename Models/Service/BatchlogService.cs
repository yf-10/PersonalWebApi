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

    /// <summary>
    /// BatchlogMainを条件で取得
    /// </summary>
    public List<BatchlogMain> GetBatchlogs(string? uuid, string? keyword, string? status)
    {
        var worker = new PostgresDbWorker(_configuration);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        return repository.GetByCondition(uuid, keyword, status);
    }

    /// <summary>
    /// バッチログ開始（UUID生成、BatchlogMain/Detail登録）
    /// </summary>
    public string BeginBatchlog(string programId, string programName, string? userName)
    {
        var uuid = Guid.NewGuid().ToString();

        var batchlogMain = new BatchlogMain(
            uuid,
            programId,
            programName,
            "STARTED",
            DateTime.Now,
            null,
            [],
            userName
        );

        var batchlogDetail = new BatchlogDetail(
            uuid, // batchlogUuid
            0,    // logNo
            "Batch started.", // logMsg
            DateTime.Now,     // logTime
            userName          // userName
        );

        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        RegisterBatchlogWithTransaction(batchlogMain);
        RegisterBatchlogDetailWithTransaction(batchlogDetail);
        scope.Complete();

        return uuid;
    }

    /// <summary>
    /// バッチログ完了
    /// </summary>
    public void CompleteBatchlog(string uuid, string? userName)
    {
        var worker = new PostgresDbWorker(_configuration);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        var batchlog = repository.GetByUuid(uuid);
        if (batchlog != null) {
            batchlog.Status = "COMPLETED";
            batchlog.EndTime = DateTime.Now;
            batchlog.UserName = userName;
            UpdateBatchlogWithTransaction(batchlog);

            var detail = new BatchlogDetail(
                uuid,           // batchlogUuid
                0,              // logNo
                "Batch completed.", // logMsg
                DateTime.Now,   // logTime
                userName        // userName
            );
            RegisterBatchlogDetailWithTransaction(detail);
        }
    }

    /// <summary>
    /// バッチログ中断
    /// </summary>
    public void AbortBatchlog(string uuid, string? userName)
    {
        var worker = new PostgresDbWorker(_configuration);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        var batchlog = repository.GetByUuid(uuid);
        if (batchlog != null)
        {
            batchlog.Status = "ABORTED";
            batchlog.EndTime = DateTime.Now;
            batchlog.UserName = userName;
            UpdateBatchlogWithTransaction(batchlog);

            var detail = new BatchlogDetail(
                uuid,           // batchlogUuid
                0,              // logNo
                "Batch aborted.", // logMsg
                DateTime.Now,   // logTime
                userName        // userName
            );
            RegisterBatchlogDetailWithTransaction(detail);
        }
    }

    /// <summary>
    /// バッチログ詳細追加
    /// </summary>
    public void AddBatchlogLog(BatchlogDetail detail, string? userName) {
        detail.LogTime = DateTime.Now;
        detail.UserName = userName;
        RegisterBatchlogDetailWithTransaction(detail);
    }
}