namespace PersonalWebApi.Utilities;

/// <summary>
/// Interface for database worker operations.
/// Provides methods for executing SQL commands, transactions, and retrieving query results.
/// </summary>
public interface IDbWorker {

    /// <summary>
    /// Executes a SQL command (such as INSERT, UPDATE, DELETE) synchronously with parameters.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="prms">The collection of parameters for the SQL command. Can be null if no parameters are needed.</param>
    /// <returns>The number of rows affected.</returns>
    int ExecuteSql(string sql, QueryParameterCollection? prms);

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
    /// <param name="prms">The collection of parameters for the SQL command. Can be null if no parameters are needed.</param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected as result.</returns>
    Task<int> ExecuteSqlAsync(string sql, QueryParameterCollection? prms);

    /// <summary>
    /// Executes a SQL command (such as INSERT, UPDATE, DELETE) asynchronously without parameters.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected as result.</returns>
    Task<int> ExecuteSqlAsync(string sql);

    /// <summary>
    /// Executes a SQL query synchronously and returns the result as a QueryResult.
    /// Each row is represented as a dictionary with column names as keys and column values as values.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="prms">The collection of parameters for the SQL query. Can be null if no parameters are needed.</param>
    /// <returns>A QueryResult representing the result set.</returns>
    QueryResult ExecuteSqlGetList(string sql, QueryParameterCollection? prms);

    /// <summary>
    /// Executes a SQL query synchronously and returns the result as a QueryResult.
    /// Each row is represented as a dictionary with column names as keys and column values as values.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <returns>A QueryResult representing the result set.</returns>
    QueryResult ExecuteSqlGetList(string sql);

    /// <summary>
    /// Executes a SQL query asynchronously and returns the result as a QueryResult.
    /// Each row is represented as a dictionary with column names as keys and column values as values.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="prms">The collection of parameters for the SQL query. Can be null if no parameters are needed.</param>
    /// <returns>A task representing the asynchronous operation, with a QueryResult representing the result set.</returns>
    Task<QueryResult> ExecuteSqlGetListAsync(string sql, QueryParameterCollection? prms);

    /// <summary>
    /// Executes a SQL query asynchronously and returns the result as a QueryResult.
    /// Each row is represented as a dictionary with column names as keys and column values as values.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a QueryResult representing the result set.</returns>
    Task<QueryResult> ExecuteSqlGetListAsync(string sql);

}