using System.Transactions;
using Microsoft.Extensions.Options;
using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.DataAccess;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.Service;
/// --------------------------------------------------------------------------------
/// <summary>
/// 保有株式サービスクラス
/// </summary>
/// --------------------------------------------------------------------------------
public class StockService(ILogger logger, IOptions<AppSettings> options) : BaseService(logger, options) {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 保有株式情報：全件取得
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<Stock> GetAll() {
        var manager = new PostgresManager(_logger, _options);
        var repository = new StockRepository(manager);
        return repository.Select();
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 保有株式情報：全件取得（非同期）
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public async Task<List<Stock>> GetAllAsync() {
        var manager = new PostgresManager(_logger, _options);
        var repository = new StockRepository(manager);
        return await repository.SelectAsync();
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 保有株式情報：証券コード指定で取得
    /// </summary>
    /// <param name="code">証券コード</param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public Stock? GetByCode(string code) {
        var manager = new PostgresManager(_logger, _options);
        var repository = new StockRepository(manager);
        return repository.SelectByCode(code);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 保有株式データ登録・更新（主キー重複時は更新、なければ登録）
    /// </summary>
    /// <param name="stocks"></param>
    /// <returns>登録・更新件数</returns>
    /// --------------------------------------------------------------------------------
    public int InsertOrUpdateAll(List<Stock> stocks) {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var manager = new PostgresManager(_logger, _options);
        var repository = new StockRepository(manager);
        int count = 0;
        foreach (var stock in stocks) {
            count += repository.InsertOrUpdate(stock);
        }
        scope.Complete();
        return count;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 保有株式データを削除
    /// </summary>
    /// <param name="code">証券コード</param>
    /// <returns>削除件数</returns>
    /// --------------------------------------------------------------------------------
    public int Delete(string code) {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var manager = new PostgresManager(_logger, _options);
        var repository = new StockRepository(manager);
        int count = repository.DeleteByCode(code);
        scope.Complete();
        return count;
    }

}