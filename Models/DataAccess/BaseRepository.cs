using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;
/// --------------------------------------------------------------------------------
/// <summary>
/// データアクセス用の基底リポジトリクラス
/// </summary>
/// <typeparam name="T">エンティティ型</typeparam>
/// --------------------------------------------------------------------------------
public abstract class BaseRepository<T> {
    protected readonly PostgresManager _manager;
    protected readonly IEntityMapper<T> _mapper;
    protected readonly QueryParameterCollection _parameters = [];

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// コンストラクタ：レポジトリ初期化
    /// </summary>
    /// <param name="manager">データベースマネージャー</param>
    /// --------------------------------------------------------------------------------
    protected BaseRepository(PostgresManager manager) {
        _manager = manager;
        _mapper = CreateMapper();
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// エンティティマッパーのインスタンスを生成するためにオーバーライド
    /// </summary>
    /// <returns>IEntityMapperのインスタンス</returns>
    protected abstract IEntityMapper<T> CreateMapper();
    /// --------------------------------------------------------------------------------

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// SQLパラメータ初期化
    /// </summary>
    /// --------------------------------------------------------------------------------
    protected void InitializeParameters() {
        _parameters.Clear();
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// SQLパラメータ追加
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// --------------------------------------------------------------------------------
    protected void AddParameter(string name, object? value) {
        _parameters.Add(new QueryParameter(name, value));
    }

}