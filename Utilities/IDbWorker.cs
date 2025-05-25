namespace PersonalWebApi.Utilities;
/// --------------------------------------------------------------------------------
/// <summary>
/// データベース操作用ワーカーのインターフェース
/// SQLコマンドの実行、トランザクション、クエリ結果の取得などのメソッドを提供する
/// </summary>
/// --------------------------------------------------------------------------------
public interface IDbWorker {

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// パラメータ付きでSQLコマンド（INSERT, UPDATE, DELETEなど）を同期実行する
    /// </summary>
    /// <param name="sql">実行するSQLコマンド</param>
    /// <param name="prms">SQLコマンドのパラメータコレクション。不要な場合はnull可</param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    int ExecuteSql(string sql, QueryParameterCollection? prms);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// パラメータなしでSQLコマンド（INSERT, UPDATE, DELETEなど）を同期実行する
    /// </summary>
    /// <param name="sql">実行するSQLコマンド</param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    int ExecuteSql(string sql);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// パラメータ付きでSQLコマンド（INSERT, UPDATE, DELETEなど）を非同期実行する
    /// </summary>
    /// <param name="sql">実行するSQLコマンド</param>
    /// <param name="prms">SQLコマンドのパラメータコレクション。不要な場合はnull可</param>
    /// <returns>非同期操作のタスク。結果は影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    Task<int> ExecuteSqlAsync(string sql, QueryParameterCollection? prms);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// パラメータなしでSQLコマンド（INSERT, UPDATE, DELETEなど）を非同期実行する
    /// </summary>
    /// <param name="sql">実行するSQLコマンド</param>
    /// <returns>非同期操作のタスク。結果は影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    Task<int> ExecuteSqlAsync(string sql);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// パラメータ付きでSQLクエリを同期実行し、結果をQueryResultとして返す
    /// 各行は列名と値の辞書で表現される
    /// </summary>
    /// <param name="sql">実行するSQLクエリ</param>
    /// <param name="prms">SQLクエリのパラメータコレクション。不要な場合はnull可</param>
    /// <returns>結果セット（QueryResult）</returns>
    /// --------------------------------------------------------------------------------
    QueryResult ExecuteSqlGetList(string sql, QueryParameterCollection? prms);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// パラメータなしでSQLクエリを同期実行し、結果をQueryResultとして返す
    /// 各行は列名と値の辞書で表現される
    /// </summary>
    /// <param name="sql">実行するSQLクエリ</param>
    /// <returns>結果セット（QueryResult）</returns>
    /// --------------------------------------------------------------------------------
    QueryResult ExecuteSqlGetList(string sql);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// パラメータ付きでSQLクエリを非同期実行し、結果をQueryResultとして返す
    /// 各行は列名と値の辞書で表現される
    /// </summary>
    /// <param name="sql">実行するSQLクエリ</param>
    /// <param name="prms">SQLクエリのパラメータコレクション。不要な場合はnull可</param>
    /// <returns>非同期操作のタスク。結果はQueryResult</returns>
    /// --------------------------------------------------------------------------------
    Task<QueryResult> ExecuteSqlGetListAsync(string sql, QueryParameterCollection? prms);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// パラメータなしでSQLクエリを非同期実行し、結果をQueryResultとして返す
    /// 各行は列名と値の辞書で表現される
    /// </summary>
    /// <param name="sql">実行するSQLクエリ</param>
    /// <returns>非同期操作のタスク。結果はQueryResult</returns>
    /// --------------------------------------------------------------------------------
    Task<QueryResult> ExecuteSqlGetListAsync(string sql);

}