using System.Transactions;

using Microsoft.Extensions.Options;

using PersonalWebApi.Models.Config;
using PersonalWebApi.Models.Data;
using PersonalWebApi.Models.DataAccess;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.Service;
/// --------------------------------------------------------------------------------
/// <summary>
/// サービスクラス
/// </summary>
/// --------------------------------------------------------------------------------
public class SalaryService(ILogger logger, IOptions<AppSettings> options) : BaseService(logger, options) {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 給与情報：全件取得
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<Salary> GetAll() {
        var manager = new PostgresManager(_logger, _options);
        var repository = new SalaryRepository(manager);
        return repository.Select();
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 給与情報：全件取得（非同期）
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public async Task<List<Salary>> GetAllAsync() {
        var manager = new PostgresManager(_logger, _options);
        var repository = new SalaryRepository(manager);
        return await repository.SelectAsync();
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 給与情報：指定した年月のデータを取得する
    /// </summary>
    /// <param name="startYm">開始年月（YYYYMM）</param>
    /// <param name="endYm">終了年月（YYYYMM）</param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<Salary> GetByMonthBetween(string? startYm, string? endYm) {
        var manager = new PostgresManager(_logger, _options);
        var repository = new SalaryRepository(manager);
        return repository.SelectByMonthBetween(startYm, endYm);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 給与データ登録（主キー重複時は更新、なければ登録）
    /// </summary>
    /// <param name="salaries"></param>
    /// <returns>登録・更新件数</returns>
    /// --------------------------------------------------------------------------------
    public int InsertOrUpdateAll(List<Salary> salaries) {
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        var manager = new PostgresManager(_logger, _options);
        var repository = new SalaryRepository(manager);
        int count = repository.InsertOrUpdateAll(salaries);
        scope.Complete();
        return count;
    }

}