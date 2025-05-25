using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using PersonalWebApi.Models.Config;

namespace PersonalWebApi.Utilities;
/// --------------------------------------------------------------------------------
/// <summary>
/// コマンドを実行するユーティリティクラス for PostgreSQL
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
/// --------------------------------------------------------------------------------
public class PostgresManager(ILogger logger, IOptions<AppSettings> options) : IDbWorker {
    private readonly ILogger _logger = logger;
    private readonly string _connectionString = options.Value.Database.PostgresConStr ?? throw new InvalidOperationException("Connection string is not configured.");

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// パラメータを追加する
    /// </summary>
    /// <param name="command">パラメータを追加するコマンド</param>
    /// <param name="prms">追加するパラメータのコレクション</param>
    /// --------------------------------------------------------------------------------
    private static void AddParameters(NpgsqlCommand command, QueryParameterCollection? prms) {
        if (prms == null || prms.Count == 0) return;
        var paramNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var prm in prms) {
            if (!paramNames.Add(prm.Name))
                throw new ArgumentException($"Duplicate parameter name: {prm.Name}");
            command.Parameters.Add(new NpgsqlParameter(prm.Name, prm.Type) { Value = prm.Value ?? DBNull.Value });
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// パラメータをログ用に整形する
    /// </summary>
    /// <param name="prms">整形するパラメータのコレクション</param>
    /// <returns>パラメータの文字列表現</returns>
    /// --------------------------------------------------------------------------------
    private static string FormatParameters(QueryParameterCollection? prms) {
        if (prms == null || prms.Count == 0) return "(none)";
        return string.Join(", ", prms.Select(p => $"{p.Name}={p.Value}({p.Type})"));
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 指定された実行関数でSQLコマンドを同期実行する
    /// </summary>
    /// <typeparam name="T">実行関数の戻り値の型</typeparam>
    /// <param name="sql">実行するSQL</param>
    /// <param name="prms">SQLのパラメータ</param>
    /// <param name="executor">コマンドを実行し結果を返す関数</param>
    /// <returns>実行関数の戻り値</returns>
    /// --------------------------------------------------------------------------------
    private T ExecuteCommand<T>(string sql, QueryParameterCollection? prms, Func<NpgsqlCommand, T> executor) {
        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("SQL must not be null or empty.", nameof(sql));
        using var conn = new NpgsqlConnection(_connectionString);
        try {
            conn.Open();
            using var command = new NpgsqlCommand(sql, conn);
            AddParameters(command, prms);
            _logger.LogDebug("Executing SQL:\n{Sql}\nParams: {Params}", sql, FormatParameters(prms));
            return executor(command);
        } catch (Exception ex) {
            _logger.LogError(ex, "Exception occured while executing SQL:\n{Sql}\nParams: {Params}", sql, FormatParameters(prms));
            throw;
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 指定された実行関数でSQLコマンドを非同期実行する
    /// </summary>
    /// <typeparam name="T">実行関数の戻り値の型</typeparam>
    /// <param name="sql">実行するSQL</param>
    /// <param name="prms">SQLのパラメータ</param>
    /// <param name="executor">コマンドを実行し結果を返す非同期関数</param>
    /// <returns>実行関数の戻り値</returns>
    /// --------------------------------------------------------------------------------
    private async Task<T> ExecuteCommandAsync<T>(string sql, QueryParameterCollection? prms, Func<NpgsqlCommand, Task<T>> executor) {
        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("SQL must not be null or empty.", nameof(sql));
        await using var conn = new NpgsqlConnection(_connectionString);
        try {
            await conn.OpenAsync();
            await using var command = new NpgsqlCommand(sql, conn);
            AddParameters(command, prms);
            _logger.LogDebug("Executing SQL:\n{Sql}\nParams: {Params}", sql, FormatParameters(prms));
            return await executor(command);
        } catch (Exception ex) {
            _logger.LogError(ex, "Exception occured while executing SQL:\n{Sql}\nParams: {Params}", sql, FormatParameters(prms));
            throw;
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// データを返さないSQLコマンドを実行する
    /// </summary>
    /// <param name="sql">実行するSQL</param>
    /// <param name="prms">SQLのパラメータ</param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int ExecuteSql(string sql, QueryParameterCollection? prms)
        => ExecuteCommand(sql, prms, cmd => cmd.ExecuteNonQuery());

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// データを返さないSQLコマンドを実行する
    /// </summary>
    /// <param name="sql">実行するSQL</param>
    /// <returns>影響を受けた行数</returns>
    /// --------------------------------------------------------------------------------
    public int ExecuteSql(string sql)
        => ExecuteSql(sql, null);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// データを返さないSQLコマンドを非同期で実行する
    /// </summary>
    /// <param name="sql">実行するSQL</param>
    /// <param name="prms">SQLのパラメータ</param>
    /// <returns>影響を受けた行数（非同期）</returns>
    /// --------------------------------------------------------------------------------
    public Task<int> ExecuteSqlAsync(string sql, QueryParameterCollection? prms)
        => ExecuteCommandAsync(sql, prms, cmd => cmd.ExecuteNonQueryAsync());

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// データを返さないSQLコマンドを非同期で実行する
    /// </summary>
    /// <param name="sql">実行するSQL</param>
    /// <returns>影響を受けた行数（非同期）</returns>
    /// --------------------------------------------------------------------------------
    public Task<int> ExecuteSqlAsync(string sql)
        => ExecuteSqlAsync(sql, null);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 複数のSQLコマンドをトランザクションで実行する
    /// </summary>
    /// <param name="commands">トランザクションで実行するSQLとパラメータのコレクション</param>
    /// --------------------------------------------------------------------------------
    public void ExecuteTransaction(IEnumerable<(string sql, QueryParameterCollection? parameters)> commands)
        => ExecuteTransaction(commands, IsolationLevel.ReadCommitted);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 複数のSQLコマンドを指定した分離レベルでトランザクション実行する
    /// </summary>
    /// <param name="commands">トランザクションで実行するSQLとパラメータのコレクション</param>
    /// <param name="isolationLevel">トランザクション分離レベル</param>
    /// --------------------------------------------------------------------------------
    public void ExecuteTransaction(IEnumerable<(string sql, QueryParameterCollection? parameters)> commands, IsolationLevel isolationLevel) {
        ArgumentNullException.ThrowIfNull(commands);
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var transaction = conn.BeginTransaction(isolationLevel);
        try {
            foreach (var (sql, prms) in commands) {
                if (string.IsNullOrWhiteSpace(sql))
                    throw new ArgumentException("SQL must not be null or empty.");
                using var command = new NpgsqlCommand(sql, conn, transaction);
                AddParameters(command, prms);
                command.ExecuteNonQuery();
            }
            transaction.Commit();
        } catch (Exception ex) {
            try {
                if (transaction.Connection != null)
                    transaction.Rollback();
            } catch (Exception rollbackEx) {
                _logger.LogError(rollbackEx, "Exception occured while rolling back transaction");
            }
            var logDetails = string.Join("\n", commands.Select(c =>
                $"SQL: {c.sql}\nParams: {FormatParameters(c.parameters)}"));
            _logger.LogError(ex, "Exception occured while executing transaction.\n{log}", logDetails);
            throw;
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 複数のSQLコマンドを非同期でトランザクション実行する
    /// </summary>
    /// <param name="commands">トランザクションで実行するSQLとパラメータのコレクション</param>
    /// <returns>非同期タスク</returns>
    /// --------------------------------------------------------------------------------
    public Task ExecuteTransactionAsync(IEnumerable<(string sql, QueryParameterCollection? parameters)> commands)
        => ExecuteTransactionAsync(commands, IsolationLevel.ReadCommitted);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 複数のSQLコマンドを指定した分離レベルで非同期トランザクション実行する
    /// </summary>
    /// <param name="commands">トランザクションで実行するSQLとパラメータのコレクション</param>
    /// <param name="isolationLevel">トランザクション分離レベル</param>
    /// <returns>非同期タスク</returns>
    /// --------------------------------------------------------------------------------
    public async Task ExecuteTransactionAsync(
        IEnumerable<(string sql, QueryParameterCollection? parameters)> commands,
        IsolationLevel isolationLevel
    ) {
        ArgumentNullException.ThrowIfNull(commands);
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync(isolationLevel);
        try {
            foreach (var (sql, prms) in commands) {
                if (string.IsNullOrWhiteSpace(sql))
                    throw new ArgumentException("SQL must not be null or empty.");
                await using var command = new NpgsqlCommand(sql, conn, transaction);
                AddParameters(command, prms);
                await command.ExecuteNonQueryAsync();
            }
            await transaction.CommitAsync();
        } catch (Exception ex) {
            try {
                if (transaction.Connection != null)
                    await transaction.RollbackAsync();
            } catch (Exception rollbackEx) {
                _logger.LogError(rollbackEx, "Exception occured while rolling back transaction");
            }
            var logDetails = string.Join("\n", commands.Select(c =>
                $"SQL: {c.sql}\nParams: {FormatParameters(c.parameters)}"));
            _logger.LogError(ex, "Exception occured while executing transaction.\n{log}", logDetails);
            throw;
        }
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// SQLクエリを実行し、結果を辞書のリストとして返す
    /// </summary>
    /// <param name="sql">実行するSQLクエリ</param>
    /// <param name="prms">SQLのパラメータ</param>
    /// <returns>結果セット（各行は列名と値の辞書）</returns>
    /// --------------------------------------------------------------------------------
    public QueryResult ExecuteSqlGetList(string sql, QueryParameterCollection? prms) {
        return ExecuteCommand(sql, prms, static cmd => {
            var results = new QueryResult();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++) {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                results.Add(row);
            }
            return results;
        });
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// SQLクエリを実行し、結果を辞書のリストとして返す
    /// </summary>
    /// <param name="sql">実行するSQLクエリ</param>
    /// <returns>結果セット（各行は列名と値の辞書）</returns>
    /// --------------------------------------------------------------------------------
    public QueryResult ExecuteSqlGetList(string sql)
        => ExecuteSqlGetList(sql, null);

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// SQLクエリを非同期で実行し、結果を辞書のリストとして返す
    /// </summary>
    /// <param name="sql">実行するSQLクエリ</param>
    /// <param name="prms">SQLのパラメータ</param>
    /// <returns>結果セット（各行は列名と値の辞書）の非同期タスク</returns>
    /// --------------------------------------------------------------------------------
    public async Task<QueryResult> ExecuteSqlGetListAsync(string sql, QueryParameterCollection? prms) {
        return await ExecuteCommandAsync(sql, prms, async cmd => {
            var results = new QueryResult();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++) {
                    row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                }
                results.Add(row);
            }
            return results;
        });
    }

    /// --------------------------------------------------------------------------------
    /// <summary>
    /// SQLクエリを非同期で実行し、結果を辞書のリストとして返す
    /// </summary>
    /// <param name="sql">実行するSQLクエリ</param>
    /// <returns>結果セット（各行は列名と値の辞書）の非同期タスク</returns>
    /// --------------------------------------------------------------------------------
    public Task<QueryResult> ExecuteSqlGetListAsync(string sql)
        => ExecuteSqlGetListAsync(sql, null);

}
