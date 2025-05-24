using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// リポジトリクラス
/// </summary>
/// <param name="manager"></param>
/// --------------------------------------------------------------------------------
public class BatchlogDetailRepository(PostgresManager manager) : BaseRepository<BatchlogDetail>(manager), IRepository<BatchlogDetail> {
    private readonly BatchlogDetailSqlHelper _helper = new();

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// エンティティマッパーを生成する
    /// </summary>
    /// <returns>エンティティマッパー</returns>
    /// --------------------------------------------------------------------------------
    protected override IEntityMapper<BatchlogDetail> CreateMapper() => new BatchlogDetailMapper();

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 全件取得
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<BatchlogDetail> Select() {
        var rows = _manager.ExecuteSqlGetList(_helper.GetSelectSql());
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 全件取得（非同期）
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public async Task<List<BatchlogDetail>> SelectAsync() {
        var rows = await _manager.ExecuteSqlGetListAsync(_helper.GetSelectSql());
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] バッチ実行識別子指定で取得
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<BatchlogDetail> Select(string uuid) {
        InitializeParameters();
        AddParameter("uuid", uuid);
        var rows = _manager.ExecuteSqlGetList(_helper.GetSelectByUuidSql(), _parameters);
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int Insert(BatchlogDetail entity) {
        return _manager.ExecuteSql(_helper.GetInsertSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録（非同期）
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public Task<int> InsertAsync(BatchlogDetail entity) {
        return _manager.ExecuteSqlAsync(_helper.GetInsertSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] バッチ実行識別子を指定して新規ログ番号を採番する
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns>新規ログ番号</returns>
    /// --------------------------------------------------------------------------------
    public int GetNextLogNo(string uuid) {
        InitializeParameters();
        AddParameter("uuid", uuid);
        var rows = _manager.ExecuteSqlGetList(_helper.GetLatestSql(), _parameters);
        return rows.Count > 0 ? _mapper.MapRowToEntity(rows[0]).LogNo + 1 : 0;
    }

}