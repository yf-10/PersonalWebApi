using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// リポジトリクラス
/// </summary>
/// <param name="manager"></param>
/// --------------------------------------------------------------------------------
public class SalaryRepository(PostgresManager manager) : BaseRepository<Salary>(manager), IRepository<Salary> {
    private readonly SalarySqlHelper _helper = new();

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// エンティティマッパーを生成する
    /// </summary>
    /// <returns>エンティティマッパー</returns>
    /// --------------------------------------------------------------------------------
    protected override IEntityMapper<Salary> CreateMapper() => new SalaryMapper();

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 全件取得
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<Salary> Select() {
        var rows = _manager.ExecuteSqlGetList(_helper.GetSelectSql());
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 全件取得（非同期）
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public async Task<List<Salary>> SelectAsync() {
        var rows = await _manager.ExecuteSqlGetListAsync(_helper.GetSelectSql());
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 期間指定で取得（開始月と終了月を指定）YYYYMM形式
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<Salary> SelectByMonthBetween(string? startYm = null, string? endYm = null) {
        if (string.IsNullOrEmpty(startYm))
            startYm = DateTime.Now.AddMonths(-12).ToString("yyyyMM");
        if (string.IsNullOrEmpty(endYm))
            endYm = DateTime.Now.ToString("yyyyMM");
        InitializeParameters();
        AddParameter("start_month", startYm);
        AddParameter("end_month", endYm);
        var rows = _manager.ExecuteSqlGetList(_helper.GetSelectByMonthBetweenSql(), _parameters);
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int Insert(Salary entity) {
        return _manager.ExecuteSql(_helper.GetInsertSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録（非同期）
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public Task<int> InsertAsync(Salary entity) {
        return _manager.ExecuteSqlAsync(_helper.GetInsertSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録
    /// </summary>
    /// <param name="entities"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int InsertAll(IEnumerable<Salary> entities) {
        int count = 0;
        foreach (var entity in entities) {
            count += Insert(entity);
        }
        return count;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録（非同期）
    /// </summary>
    /// <param name="entities"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public async Task<int> InsertAllAsync(IEnumerable<Salary> entities) {
        int count = 0;
        foreach (var entity in entities) {
            count += await InsertAsync(entity);
        }
        return count;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT or UPDATE] 主キーが重複していれば更新、なければ登録（PostgreSQL用）
    /// </summary>
    /// <param name="entities"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int InsertOrUpdateAll(IEnumerable<Salary> entities) {
        int count = 0;
        foreach (var entity in entities) {
            count += _manager.ExecuteSql(_helper.GetInsertOrUpdateSql(), _helper.ToParameterCollection(entity));
        }
        return count;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT or UPDATE] 主キーが重複していれば更新、なければ登録（非同期・PostgreSQL用）
    /// </summary>
    /// <param name="entities"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public async Task<int> InsertOrUpdateAllAsync(IEnumerable<Salary> entities) {
        int count = 0;
        foreach (var entity in entities) {
            count += await _manager.ExecuteSqlAsync(_helper.GetInsertOrUpdateSql(), _helper.ToParameterCollection(entity));
        }
        return count;
    }

}