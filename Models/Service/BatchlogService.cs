using System.Transactions;

using Microsoft.Extensions.Options;

using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.DataAccess;
using PersonalWebApi.Models.Config;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.Service;
/// --------------------------------------------------------------------------------
/// <summary>
/// サービスクラス
/// </summary>
/// --------------------------------------------------------------------------------
public class BatchlogService(ILogger logger, IOptions<AppSettings> options) : BaseService(logger, options) {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチログ取得：全件
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<Batchlog> GetAll() {
        var manager = new PostgresManager(_logger, _options);
        var batchlogMainRepository = new BatchlogMainRepository(manager);
        var batchlogDetailRepository = new BatchlogDetailRepository(manager);
        var batchlogMains = batchlogMainRepository.Select();
        var batchlogDetails = batchlogDetailRepository.Select();
        var batchlogs = new List<Batchlog>();
        batchlogMains.ForEach(main => {
            var details = batchlogDetails.Where(d => d.Uuid == main.Uuid).ToList();
            batchlogs.Add(new Batchlog(main, details));
        });
        return batchlogs;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチログ取得：バッチ実行識別子指定
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public Batchlog? Get(string uuid) {
        var manager = new PostgresManager(_logger, _options);
        var batchlogMainRepository = new BatchlogMainRepository(manager);
        var batchlogDetailRepository = new BatchlogDetailRepository(manager);
        var batchlogMain = batchlogMainRepository.Select(uuid);
        if (batchlogMain == null) return null;
        var batchlogDetails = batchlogDetailRepository.Select(uuid);
        var batchlog = new Batchlog(batchlogMain, batchlogDetails);
        return batchlog;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチログ取得：条件指定検索
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<Batchlog> Search(string? keyword, string? status) {
        var manager = new PostgresManager(_logger, _options);
        var batchlogMainRepository = new BatchlogMainRepository(manager);
        var batchlogDetailRepository = new BatchlogDetailRepository(manager);
        var batchlogMains = batchlogMainRepository.Select(keyword, status);
        var batchlogDetails = batchlogDetailRepository.Select();
        var batchlogs = new List<Batchlog>();
        batchlogMains.ForEach(main => {
            var details = batchlogDetails.Where(d => d.Uuid == main.Uuid).ToList();
            batchlogs.Add(new Batchlog(main, details));
        });
        return batchlogs;
    }
    
    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチ処理ログ登録（開始）
    /// </summary>
    /// <param name="programId"></param>
    /// <param name="programName"></param>
    /// <param name="userName"></param>
    /// <returns>採番されたバッチ実行識別子</returns>
    /// --------------------------------------------------------------------------------
    public string Begin(string programId, string? programName = "undefined", string? userName = "unknown") {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var manager = new PostgresManager(_logger, _options);
        var batchlogMainRepository = new BatchlogMainRepository(manager);
        var batchlogDetailRepository = new BatchlogDetailRepository(manager);
        var uuid = Guid.NewGuid().ToString();
        var batchlogMain = new BatchlogMain(
            uuid,
            BatchlogStatus.Running,
            programId,
            programName,
            createdBy: userName,
            updatedBy: userName
        );
        var batchlogDetail = new BatchlogDetail(
            uuid,
            logNo: 0,
            logMsg: "バッチ処理を開始しました。",
            createdBy: userName,
            updatedBy: userName
        );
        _ = batchlogMainRepository.Insert(batchlogMain);
        _ = batchlogDetailRepository.Insert(batchlogDetail);
        scope.Complete();
        return uuid;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチ処理ログ登録（完了）
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="userName"></param>
    /// <returns>ログ番号</returns>
    /// --------------------------------------------------------------------------------
    public int Complete(string uuid, string? userName = "unknown") {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var manager = new PostgresManager(_logger, _options);
        var batchlogMainRepository = new BatchlogMainRepository(manager);
        var batchlogDetailRepository = new BatchlogDetailRepository(manager);
        var batchlogMain = batchlogMainRepository.Select(uuid) ?? throw new Exception("BatchlogMain not found.");
        batchlogMain.Status = BatchlogStatus.Complete;
        batchlogMain.EndTime = DateTime.Now;
        int logNo = batchlogDetailRepository.GetNextLogNo(uuid);
        var batchlogDetail = new BatchlogDetail(
            uuid,
            logNo: logNo,
            logMsg: "バッチ処理を完了しました。",
            createdBy: userName,
            updatedBy: userName
        );
        _ = batchlogMainRepository.Update(batchlogMain);
        _ = batchlogDetailRepository.Insert(batchlogDetail);
        scope.Complete();
        return logNo;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチ処理ログ登録（中止）
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="userName"></param>
    /// <returns>ログ番号</returns>
    /// --------------------------------------------------------------------------------
    public int Abort(string uuid, string? userName = "unknown") {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var manager = new PostgresManager(_logger, _options);
        var batchlogMainRepository = new BatchlogMainRepository(manager);
        var batchlogDetailRepository = new BatchlogDetailRepository(manager);
        var batchlogMain = batchlogMainRepository.Select(uuid) ?? throw new Exception("BatchlogMain not found.");
        batchlogMain.Status = BatchlogStatus.Abort;
        batchlogMain.EndTime = DateTime.Now;
        int logNo = batchlogDetailRepository.GetNextLogNo(uuid);
        var batchlogDetail = new BatchlogDetail(
            uuid,
            logNo: logNo,
            logMsg: "バッチ処理を中止しました。",
            createdBy: userName,
            updatedBy: userName
        );
        _ = batchlogMainRepository.Update(batchlogMain);
        _ = batchlogDetailRepository.Insert(batchlogDetail);
        scope.Complete();
        return logNo;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// バッチログ詳細追加
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="logMsg"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public (string uuid, int logNo) AddDetailLog(string uuid, string? logMsg = null, string? userName = "unknown") {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var manager = new PostgresManager(_logger, _options);
        var batchlogDetailRepository = new BatchlogDetailRepository(manager);
        var logNo = batchlogDetailRepository.GetNextLogNo(uuid);
        var batchlogDetail = new BatchlogDetail(
            uuid,
            logNo: logNo,
            logMsg: logMsg,
            createdBy: userName,
            updatedBy: userName
        );
        _ = batchlogDetailRepository.Insert(batchlogDetail);
        scope.Complete();
        return (uuid, logNo);
    }

}