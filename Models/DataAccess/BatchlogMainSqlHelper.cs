using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// SQL and parameter generation helper for BatchlogMainRepository.
/// Implements ISqlHelper for BatchlogMain entity.
/// </summary>
public class BatchlogMainSqlHelper : ISqlHelper<BatchlogMain> {
    // Table name for BatchlogMain
    private const string TableName = "batchlog_main";

    // SQL columns used in SELECT statements
    private const string SelectColumns = @"
        uuid,
        status,
        program_id,
        program_name,
        start_time,
        end_time";

    /// <summary>
    /// Gets the SQL statement to select all BatchlogMain records.
    /// </summary>
    public string GetSelectAllSql() =>
        $@"SELECT {SelectColumns}
            FROM {TableName}
            ORDER BY uuid";

    /// <summary>
    /// Gets the SQL statement to select a BatchlogMain record by UUID.
    /// </summary>
    public string GetSelectByIdSql() =>
        $@"SELECT {SelectColumns}
            FROM {TableName}
            WHERE uuid = @uuid";

    /// <summary>
    /// Gets the SQL statement to insert a new BatchlogMain record.
    /// </summary>
    public string GetInsertSql() =>
        $@"INSERT INTO {TableName} (
                uuid,
                status,
                program_id,
                program_name,
                start_time,
                end_time
            ) VALUES (
                @uuid,
                @status,
                @program_id,
                @program_name,
                @start_time,
                @end_time
            )";

    /// <summary>
    /// Gets the SQL statement to update an existing BatchlogMain record.
    /// </summary>
    public string GetUpdateSql() =>
        $@"UPDATE {TableName} SET
                status = @status,
                program_id = @program_id,
                program_name = @program_name,
                start_time = @start_time,
                end_time = @end_time
            WHERE
                uuid = @uuid";

    /// <summary>
    /// Converts a BatchlogMain object to a QueryParameterCollection for SQL operations.
    /// </summary>
    /// <param name="log">The BatchlogMain object.</param>
    /// <returns>A QueryParameterCollection with parameters for SQL.</returns>
    public QueryParameterCollection ToParameterCollection(BatchlogMain log) {
        return [
            new("@uuid", log.Uuid),
            new("@status", log.Status),
            new("@program_id", log.ProgramId),
            new("@program_name", log.ProgramName),
            new("@start_time", log.StartTime),
            new("@end_time", log.EndTime)
        ];
    }

    /// <summary>
    /// Converts an ID value to a QueryParameterCollection for SQL operations.
    /// </summary>
    /// <param name="id">The ID value (UUID).</param>
    /// <returns>A QueryParameterCollection with the ID parameter.</returns>
    public QueryParameterCollection ToIdParameterCollection(object id) {
        return [
            new("@uuid", id)
        ];
    }
}