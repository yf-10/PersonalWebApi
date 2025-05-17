namespace PersonalWebApi.Utilities;

/// <summary>
/// Interface for database worker operations.  
/// Provides methods for executing SQL commands, transactions, and retrieving query results as lists of dictionaries.
/// </summary>
public interface IDBWorker
{
    /// <summary>
    /// Executes a SQL command (such as INSERT, UPDATE, DELETE) synchronously with parameters.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="prms">The list of parameters for the SQL command. Can be null if no parameters are needed.</param>
    /// <returns>The number of rows affected.</returns>
    int ExecuteSql(string sql, List<SqlParameter>? prms);

    /// <summary>
    /// Executes a SQL command (such as INSERT, UPDATE, DELETE) synchronously without parameters.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <returns>The number of rows affected.</returns>
    int ExecuteSql(string sql);

    /// <summary>
    /// Executes a SQL command (such as INSERT, UPDATE, DELETE) asynchronously with parameters.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="prms">The list of parameters for the SQL command. Can be null if no parameters are needed.</param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected as result.</returns>
    Task<int> ExecuteSqlAsync(string sql, List<SqlParameter>? prms);

    /// <summary>
    /// Executes a SQL command (such as INSERT, UPDATE, DELETE) asynchronously without parameters.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected as result.</returns>
    Task<int> ExecuteSqlAsync(string sql);

    /// <summary>
    /// Executes multiple SQL commands within a transaction synchronously using the default isolation level.
    /// Rolls back the transaction if any command fails.
    /// </summary>
    /// <param name="commands">A collection of SQL commands and their parameters to execute in the transaction.</param>
    void ExecuteTransaction(IEnumerable<(string Sql, List<SqlParameter>? Parameters)> commands);

    /// <summary>
    /// Executes multiple SQL commands within a transaction synchronously with a specified isolation level.
    /// Rolls back the transaction if any command fails.
    /// </summary>
    /// <param name="commands">A collection of SQL commands and their parameters to execute in the transaction.</param>
    /// <param name="isolationLevel">The transaction isolation level.</param>
    void ExecuteTransaction(IEnumerable<(string Sql, List<SqlParameter>? Parameters)> commands, System.Data.IsolationLevel isolationLevel);

    /// <summary>
    /// Executes multiple SQL commands within a transaction asynchronously using the default isolation level.
    /// Rolls back the transaction if any command fails.
    /// </summary>
    /// <param name="commands">A collection of SQL commands and their parameters to execute in the transaction.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteTransactionAsync(IEnumerable<(string Sql, List<SqlParameter>? Parameters)> commands);

    /// <summary>
    /// Executes multiple SQL commands within a transaction asynchronously with a specified isolation level.
    /// Rolls back the transaction if any command fails.
    /// </summary>
    /// <param name="commands">A collection of SQL commands and their parameters to execute in the transaction.</param>
    /// <param name="isolationLevel">The transaction isolation level.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteTransactionAsync(IEnumerable<(string Sql, List<SqlParameter>? Parameters)> commands, System.Data.IsolationLevel isolationLevel);

    /// <summary>
    /// Executes a SQL query and returns the result as a list of dictionaries synchronously.
    /// Each dictionary represents a row, with column names as keys and column values as values.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="prms">The list of parameters for the SQL query. Can be null if no parameters are needed.</param>
    /// <returns>A list of dictionaries representing the result set.</returns>
    List<Dictionary<string, object>> ExecuteSqlGetList(string sql, List<SqlParameter>? prms);

    /// <summary>
    /// Executes a SQL query and returns the result as a list of dictionaries synchronously.
    /// Each dictionary represents a row, with column names as keys and column values as values.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <returns>A list of dictionaries representing the result set.</returns>
    List<Dictionary<string, object>> ExecuteSqlGetList(string sql);

    /// <summary>
    /// Executes a SQL query asynchronously and returns the result as a list of dictionaries.
    /// Each dictionary represents a row, with column names as keys and column values as values.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="prms">The list of parameters for the SQL query. Can be null if no parameters are needed.</param>
    /// <returns>A task representing the asynchronous operation, with a list of dictionaries representing the result set.</returns>
    Task<List<Dictionary<string, object>>> ExecuteSqlGetListAsync(string sql, List<SqlParameter>? prms);

    /// <summary>
    /// Executes a SQL query asynchronously and returns the result as a list of dictionaries.
    /// Each dictionary represents a row, with column names as keys and column values as values.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a list of dictionaries representing the result set.</returns>
    Task<List<Dictionary<string, object>>> ExecuteSqlGetListAsync(string sql);
}