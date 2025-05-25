using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// リポジトリクラス
/// </summary>
/// <param name="manager"></param>
/// --------------------------------------------------------------------------------
public class StockRepository(PostgresManager manager) : BaseRepository<Stock>(manager), IRepository<Stock> {
    private readonly StockSqlHelper _helper = new();

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// エンティティマッパーを生成する
    /// </summary>
    /// <returns>エンティティマッパー</returns>
    /// --------------------------------------------------------------------------------
    protected override IEntityMapper<Stock> CreateMapper() => new StockMapper();

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 全件取得
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<Stock> Select() {
        var rows = _manager.ExecuteSqlGetList(_helper.GetSelectSql());
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 全件取得（非同期）
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public async Task<List<Stock>> SelectAsync() {
        var rows = await _manager.ExecuteSqlGetListAsync(_helper.GetSelectSql());
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 主キー指定で取得
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public Stock? SelectByCode(string code) {
        InitializeParameters();
        AddParameter("code", code);
        var rows = _manager.ExecuteSqlGetList(_helper.GetSelectByIdSql(), _parameters);
        return rows.Count > 0 ? _mapper.MapRowToEntity(rows[0]) : null;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int Insert(Stock entity) {
        return _manager.ExecuteSql(_helper.GetInsertSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録（非同期）
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public Task<int> InsertAsync(Stock entity) {
        return _manager.ExecuteSqlAsync(_helper.GetInsertSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT or UPDATE] 主キーが重複していれば更新、なければ登録（PostgreSQL用）
    /// </summary>
    /// <param name="entities"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int InsertOrUpdate(Stock entity) {
        return _manager.ExecuteSql(_helper.GetInsertOrUpdateSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT or UPDATE] 主キーが重複していれば更新、なければ登録（非同期・PostgreSQL用）
    /// </summary>
    /// <param name="entities"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public Task<int> InsertOrUpdateAsync(Stock entity) {
        return _manager.ExecuteSqlAsync(_helper.GetInsertOrUpdateSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [DELETE] 主キー指定で削除
    /// </summary>
    /// <param name="code">証券コード</param>
    /// <returns>削除件数</returns>
    /// --------------------------------------------------------------------------------
    public int DeleteByCode(string code){
        InitializeParameters();
        AddParameter("code", code);
        return _manager.ExecuteSql(_helper.GetDeleteSql(), _parameters);
    }

}