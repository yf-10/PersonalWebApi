using System.Data;
using System.Collections;

namespace PersonalWebApi.Utilities;
/// --------------------------------------------------------------------------------
/// <summary>
/// SQLコマンド用のQueryParameterオブジェクトのコレクションを表すクラス
/// </summary>
/// --------------------------------------------------------------------------------
public class QueryParameterCollection : IEnumerable<QueryParameter> {
    private readonly List<QueryParameter> _parameters = [];

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 空のQueryParameterCollectionを初期化する
    /// </summary>
    /// --------------------------------------------------------------------------------
    public QueryParameterCollection() { }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 指定したパラメータでQueryParameterCollectionを初期化する
    /// </summary>
    /// <param name="parameters">初期パラメータのセット</param>
    /// --------------------------------------------------------------------------------
    public QueryParameterCollection(IEnumerable<QueryParameter> parameters) {
        _parameters.AddRange(parameters);
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// コレクションにQueryParameterを追加する
    /// </summary>
    /// <param name="parameter">追加するパラメータ</param>
    /// --------------------------------------------------------------------------------
    public void Add(QueryParameter parameter) => _parameters.Add(parameter);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// コレクションをクリアする
    /// </summary>
    /// --------------------------------------------------------------------------------
    public void Clear() => _parameters.Clear();

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// コレクション内のパラメータ数を取得する
    /// </summary>
    /// --------------------------------------------------------------------------------
    public int Count => _parameters.Count;

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 指定したインデックスのパラメータを取得する
    /// </summary>
    /// <param name="index">取得するパラメータの0始まりのインデックス</param>
    /// --------------------------------------------------------------------------------
    public QueryParameter this[int index] => _parameters[index];

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// コレクションを反復処理する列挙子を返す
    /// </summary>
    /// --------------------------------------------------------------------------------
    public IEnumerator<QueryParameter> GetEnumerator() => _parameters.GetEnumerator();

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// コレクションを反復処理する列挙子（非ジェネリック）を返す
    /// </summary>
    /// --------------------------------------------------------------------------------
    IEnumerator IEnumerable.GetEnumerator() => _parameters.GetEnumerator();

}

/// --------------------------------------------------------------------------------
/// <summary>
/// SQLコマンド用のパラメータ（DBMS非依存）を表すクラス
/// </summary>
/// --------------------------------------------------------------------------------
public class QueryParameter {

    /// <summary>
    /// パラメータ名（例: "@id"）
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// パラメータ値
    /// </summary>
    public object? Value { get; }
    /// <summary>
    /// パラメータのデータ型（ADO.NET標準）
    /// </summary>
    public DbType Type { get; }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// QueryParameterの新しいインスタンスを初期化する
    /// </summary>
    /// <param name="name">パラメータ名（例: "@id"）</param>
    /// <param name="value">パラメータ値</param>
    /// <param name="type">パラメータのデータ型</param>
    /// --------------------------------------------------------------------------------
    public QueryParameter(string name, object? value, DbType type) {
        Name = name;
        Value = value;
        Type = type;
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// QueryParameterの新しいインスタンスを初期化する（型は値から推論）
    /// </summary>
    /// <param name="name">パラメータ名（例: "@id"）</param>
    /// <param name="value">パラメータ値</param>
    /// --------------------------------------------------------------------------------
    public QueryParameter(string name, object? value) {
        Name = name;
        Value = value;
        Type = value switch {
            string => DbType.String,
            int => DbType.Int32,
            long => DbType.Int64,
            bool => DbType.Boolean,
            decimal => DbType.Decimal,
            double => DbType.Double,
            float => DbType.Single,
            DateTime => DbType.DateTime,
            null => DbType.Object,
            _ => DbType.Object
        };
    }

}