using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// リポジトリクラス
/// </summary>
/// <param name="manager"></param>
/// --------------------------------------------------------------------------------
public class BatchlogMainRepository(PostgresManager manager) : BaseRepository<BatchlogMain>(manager), IRepository<BatchlogMain> {
    private readonly BatchlogMainSqlHelper _helper = new();

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// エンティティマッパーを生成する
    /// </summary>
    /// <returns>エンティティマッパー</returns>
    /// --------------------------------------------------------------------------------
    protected override IEntityMapper<BatchlogMain> CreateMapper() => new BatchlogMainMapper();

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 全件取得
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<BatchlogMain> Select() {
        var rows = _manager.ExecuteSqlGetList(_helper.GetSelectSql());
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 全件取得（非同期）
    /// </summary>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public async Task<List<BatchlogMain>> SelectAsync() {
        var rows = await _manager.ExecuteSqlGetListAsync(_helper.GetSelectSql());
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] 条件指定で取得する
    /// </summary>
    /// <param name="keyword">プログラム名等のキーワード検索（任意）</param>
    /// <param name="status">ステータスで絞り込み（任意）</param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public List<BatchlogMain> Select(string? keyword, string? status) {
        var sql = _helper.GetSelectSql();
        InitializeParameters();
        var whereList = new List<string>();
        if (!string.IsNullOrEmpty(keyword)) {
            whereList.Add("program_name ILIKE @keyword");
            AddParameter("keyword", $"%{keyword}%");
        }
        if (!string.IsNullOrEmpty(status)) {
            whereList.Add("status = @status");
            AddParameter("status", status);
        }
        if (whereList.Count > 0) {
            sql += " WHERE " + string.Join(" AND ", whereList);
        }
        var rows = _manager.ExecuteSqlGetList(sql, _parameters);
        return [.. rows.Select(_mapper.MapRowToEntity)];
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] バッチ実行識別子指定で取得
    /// </summary>
    /// <param name="uuid">バッチ実行識別子</param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public BatchlogMain? Select(string uuid) {
        InitializeParameters();
        AddParameter("uuid", uuid);
        var rows = _manager.ExecuteSqlGetList(_helper.GetSelectByIdSql(), _parameters);
        return rows.Count > 0 ? _mapper.MapRowToEntity(rows[0]) : null;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [SELECT] バッチ実行識別子指定で取得（非同期）
    /// </summary>
    /// <param name="uuid">バッチ実行識別子</param>
    /// <returns></returns>
    /// --------------------------------------------------------------------------------
    public async Task<BatchlogMain?> SelectAsync(string uuid) {
        InitializeParameters();
        AddParameter("uuid", uuid);
        var rows = await _manager.ExecuteSqlGetListAsync(_helper.GetSelectByIdSql(), _parameters);
        return rows.Count > 0 ? _mapper.MapRowToEntity(rows[0]) : null;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int Insert(BatchlogMain entity) {
        return _manager.ExecuteSql(_helper.GetInsertSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録（非同期）
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public Task<int> InsertAsync(BatchlogMain entity) {
        return _manager.ExecuteSqlAsync(_helper.GetInsertSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [UPDATE] 既存レコード更新
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int Update(BatchlogMain entity) {
        return _manager.ExecuteSql(_helper.GetUpdateSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [UPDATE] 既存レコード更新
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public Task<int> UpdateAsync(BatchlogMain entity) {
        return _manager.ExecuteSqlAsync(_helper.GetUpdateSql(), _helper.ToParameterCollection(entity));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// [INSERT] 新規登録
    /// </summary>
    /// <param name="entities"></param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int InsertAll(IEnumerable<BatchlogMain> entities) {
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
    public async Task<int> InsertAllAsync(IEnumerable<BatchlogMain> entities) {
        int count = 0;
        foreach (var entity in entities) {
            count += await InsertAsync(entity);
        }
        return count;
    }

}