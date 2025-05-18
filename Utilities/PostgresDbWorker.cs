using System.Data;

using Npgsql;

namespace PersonalWebApi.Utilities;

/// <summary>
/// Utility class for executing PostgreSQL commands.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PostgresDbWorker"/> class.
/// </remarks>
/// <param name="configuration">The application configuration containing the PostgreSQL connection string.</param>
/// <param name="logger">The logger instance for logging SQL execution details.</param>
public class PostgresDbWorker(IConfiguration configuration) : IDbWorker {
    private readonly ILogger<PostgresDbWorker> _logger = new Logger<PostgresDbWorker>(new LoggerFactory());
    private readonly string _connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is not configured.");

    /// <summary>
    /// Adds parameters to the NpgsqlCommand.
    /// </summary>
    /// <param name="command">The command to which parameters will be added.</param>
    /// <param name="prms">The collection of parameters to add.</param>
    private static void AddParameters(NpgsqlCommand command, QueryParameterCollection? prms) {
        if (prms == null || prms.Count == 0) return;
        var paramNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var prm in prms) {
            if (!paramNames.Add(prm.Name))
                throw new ArgumentException($"Duplicate parameter name: {prm.Name}");
            command.Parameters.Add(new NpgsqlParameter(prm.Name, prm.Type) { Value = prm.Value ?? DBNull.Value });
        }
    }

    /// <summary>
    /// Formats parameters for logging.
    /// </summary>
    /// <param name="prms">The collection of parameters to format.</param>
    /// <returns>A string representation of the parameters.</returns>
    private static string FormatParameters(QueryParameterCollection? prms) {
        if (prms == null || prms.Count == 0) return "(none)";
        return string.Join(", ", prms.Select(p => $"{p.Name}={p.Value}({p.Type})"));
    }

    /// <summary>
    /// Executes a SQL command synchronously using the provided executor function.
    /// </summary>
    /// <typeparam name="T">The return type of the executor function.</typeparam>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="prms">The collection of parameters for the SQL command.</param>
    /// <param name="executor">A function that executes the command and returns a result.</param>
    /// <returns>The result of the executor function.</returns>
    private T ExecuteCommand<T>(string sql, QueryParameterCollection? prms, Func<NpgsqlCommand, T> executor) {
        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("SQL must not be null or empty.", nameof(sql));
        using var conn = new NpgsqlConnection(_connectionString);
        try {
            conn.Open();
            using var command = new NpgsqlCommand(sql, conn);
            AddParameters(command, prms);
            _logger.LogInformation("Executing SQL: {Sql}\nParams: {Params}", sql, FormatParameters(prms));
            return executor(command);
        } catch (Exception ex) {
            _logger.LogError(ex,
                "Exception occurred while executing SQL. \n SQL: {Sql} \n Params: {Params}",
                sql, FormatParameters(prms));
            throw;
        }
    }

    /// <summary>
    /// Executes a SQL command asynchronously using the provided executor function.
    /// </summary>
    /// <typeparam name="T">The return type of the executor function.</typeparam>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="prms">The collection of parameters for the SQL command.</param>
    /// <param name="executor">A function that executes the command and returns a result asynchronously.</param>
    /// <returns>A task representing the asynchronous operation, with the result of the executor function.</returns>
    private async Task<T> ExecuteCommandAsync<T>(string sql, QueryParameterCollection? prms, Func<NpgsqlCommand, Task<T>> executor) {
        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("SQL must not be null or empty.", nameof(sql));
        await using var conn = new NpgsqlConnection(_connectionString);
        try {
            await conn.OpenAsync();
            await using var command = new NpgsqlCommand(sql, conn);
            AddParameters(command, prms);
            _logger.LogDebug("Executing SQL: {Sql}\nParams: {Params}", sql, FormatParameters(prms));
            return await executor(command);
        } catch (Exception ex) {
            _logger.LogError(ex,
                "Exception occurred while executing SQL.\nSQL: {Sql}\nParams: {Params}",
                sql, FormatParameters(prms));
            throw;
        }
    }

    /// <summary>
    /// Executes a SQL command that does not return data.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="prms">The collection of parameters for the SQL command.</param>
    /// <returns>The number of rows affected.</returns>
    public int ExecuteSql(string sql, QueryParameterCollection? prms)
        => ExecuteCommand(sql, prms, cmd => cmd.ExecuteNonQuery());

    /// <summary>
    /// Executes a SQL command that does not return data.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <returns>The number of rows affected.</returns>
    public int ExecuteSql(string sql)
        => ExecuteSql(sql, null);

    /// <summary>
    /// Executes a SQL command asynchronously that does not return data.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="prms">The collection of parameters for the SQL command.</param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected as result.</returns>
    public Task<int> ExecuteSqlAsync(string sql, QueryParameterCollection? prms)
        => ExecuteCommandAsync(sql, prms, cmd => cmd.ExecuteNonQueryAsync());

    /// <summary>
    /// Executes a SQL command asynchronously that does not return data.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected as result.</returns>
    public Task<int> ExecuteSqlAsync(string sql)
        => ExecuteSqlAsync(sql, null);

    /// <summary>
    /// Executes multiple SQL commands within a transaction.
    /// </summary>
    /// <param name="commands">A collection of SQL commands and their parameters to execute in the transaction.</param>
    public void ExecuteTransaction(IEnumerable<(string sql, QueryParameterCollection? parameters)> commands)
        => ExecuteTransaction(commands, IsolationLevel.ReadCommitted);

    /// <summary>
    /// Executes multiple SQL commands within a transaction with a specified isolation level.
    /// </summary>
    /// <param name="commands">A collection of SQL commands and their parameters to execute in the transaction.</param>
    /// <param name="isolationLevel">The transaction isolation level.</param>
    public void ExecuteTransaction(IEnumerable<(string sql, QueryParameterCollection? parameters)> commands, IsolationLevel isolationLevel) {
        ArgumentNullException.ThrowIfNull(commands);
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var transaction = conn.BeginTransaction(isolationLevel);
        try {
            foreach (var (sql, prms) in commands) {
                if (string.IsNullOrWhiteSpace(sql))
                    throw new ArgumentException("SQL must not be null or empty.", nameof(sql));
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
                _logger.LogError(rollbackEx, "Rollback failed");
            }
            var logDetails = string.Join("\n", commands.Select(c =>
                $"SQL: {c.sql}\nParams: {FormatParameters(c.parameters)}"));
            _logger.LogError(ex, "Exception occurred while executing transaction.\n{0}", logDetails);
            throw;
        }
    }

    /// <summary>
    /// Executes multiple SQL commands asynchronously within a transaction.
    /// </summary>
    /// <param name="commands">A collection of SQL commands and their parameters to execute in the transaction.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ExecuteTransactionAsync(IEnumerable<(string sql, QueryParameterCollection? parameters)> commands)
        => ExecuteTransactionAsync(commands, IsolationLevel.ReadCommitted);

    /// <summary>
    /// Executes multiple SQL commands asynchronously within a transaction with a specified isolation level.
    /// </summary>
    /// <param name="commands">A collection of SQL commands and their parameters to execute in the transaction.</param>
    /// <param name="isolationLevel">The transaction isolation level.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
                    throw new ArgumentException("SQL must not be null or empty.", nameof(sql));
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
                _logger.LogError(rollbackEx, "Rollback failed");
            }
            var logDetails = string.Join("\n", commands.Select(c =>
                $"SQL: {c.sql}\nParams: {FormatParameters(c.parameters)}"));
            _logger.LogError(ex, "Exception occurred while executing transaction.\n{0}", logDetails);
            throw;
        }
    }

    /// <summary>
    /// Executes a SQL query and returns the result as a list of dictionaries.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="prms">The collection of parameters for the SQL query.</param>
    /// <returns>A QueryResult representing the result set. Each dictionary contains column names and their values for a row.</returns>
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

    /// <summary>
    /// Executes a SQL query and returns the result as a list of dictionaries.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <returns>A QueryResult representing the result set. Each dictionary contains column names and their values for a row.</returns>
    public QueryResult ExecuteSqlGetList(string sql)
        => ExecuteSqlGetList(sql, null);

    /// <summary>
    /// Executes a SQL query asynchronously and returns the result as a list of dictionaries.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="prms">The collection of parameters for the SQL query.</param>
    /// <returns>A task representing the asynchronous operation, with a QueryResult representing the result set. Each dictionary contains column names and their values for a row.</returns>
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

    /// <summary>
    /// Executes a SQL query asynchronously and returns the result as a list of dictionaries.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a QueryResult representing the result set. Each dictionary contains column names and their values for a row.</returns>
    public Task<QueryResult> ExecuteSqlGetListAsync(string sql)
        => ExecuteSqlGetListAsync(sql, null);

}
