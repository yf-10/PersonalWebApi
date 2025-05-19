using Microsoft.Extensions.Options;
using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.DataAccess;
using PersonalWebApi.Utilities;
using System.Transactions;

namespace PersonalWebApi.Models.Service;

/// <summary>
/// Service class for BatchlogMain data registration, update, and retrieval with transaction management.
/// </summary>
public class BatchlogService
{
    private readonly IOptions<AppSettings> _options;

    public BatchlogService(IOptions<AppSettings> options)
    {
        _options = options;
    }

    public int RegisterBatchlogWithTransaction(BatchlogMain batchlog)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var worker = new PostgresDbWorker(_options);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        int count = repository.Insert(batchlog);
        scope.Complete();
        return count;
    }

    public int RegisterBatchlogDetailWithTransaction(BatchlogDetail detail)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var worker = new PostgresDbWorker(_options);
        var repository = new BatchlogDetailRepository(worker, new BatchlogDetailSqlHelper(), new BatchlogDetailMapper());
        int count = repository.Insert(detail);
        scope.Complete();
        return count;
    }

    public int UpdateBatchlogWithTransaction(BatchlogMain batchlog)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var worker = new PostgresDbWorker(_options);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        int count = repository.Update(batchlog);
        scope.Complete();
        return count;
    }

    public List<BatchlogMain> GetAllBatchlogs()
    {
        var worker = new PostgresDbWorker(_options);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        return repository.GetAll();
    }

    public List<BatchlogMain> GetBatchlogs(string? uuid, string? keyword, string? status)
    {
        var worker = new PostgresDbWorker(_options);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        return repository.GetByCondition(uuid, keyword, status);
    }

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

    public void CompleteBatchlog(string uuid, string? userName)
    {
        var worker = new PostgresDbWorker(_options);
        var repository = new BatchlogMainRepository(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper());
        var batchlog = repository.GetByUuid(uuid);
        if (batchlog != null)
        {
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

    public void AbortBatchlog(string uuid, string? userName)
    {
        var worker = new PostgresDbWorker(_options);
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

    public void AddBatchlogLog(BatchlogDetail detail, string? userName)
    {
        detail.LogTime = DateTime.Now;
        detail.UserName = userName;
        RegisterBatchlogDetailWithTransaction(detail);
    }
}