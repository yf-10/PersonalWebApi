using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// SQL and parameter generation helper for BatchlogDetailRepository.
/// Implements ISqlHelper for BatchlogDetail entity.
/// </summary>
public class BatchlogDetailSqlHelper : ISqlHelper<BatchlogDetail> {
    private const string TableName = "batchlog_detail";
    private const string SelectColumns = @"
        batchlog_uuid,
        id,
        message,
        log_time";

    public string GetSelectAllSql() =>
        $@"SELECT {SelectColumns}
            FROM {TableName}
            ORDER BY batchlog_uuid, id";

    public string GetSelectByIdSql() =>
        $@"SELECT {SelectColumns}
            FROM {TableName}
            WHERE batchlog_uuid = @batchlog_uuid AND id = @id";

    public string GetInsertSql() =>
        $@"INSERT INTO {TableName} (
                batchlog_uuid,
                id,
                message,
                log_time
            ) VALUES (
                @batchlog_uuid,
                @id,
                @message,
                @log_time
            )";

    public string GetUpdateSql() =>
        $@"UPDATE {TableName} SET
                message = @message,
                log_time = @log_time
            WHERE
                batchlog_uuid = @batchlog_uuid AND id = @id";

    public QueryParameterCollection ToParameterCollection(BatchlogDetail entity) {
        return [
            new("@batchlog_uuid", entity.BatchlogUuid),
            new("@id", entity.Id),
            new("@message", entity.Message),
            new("@log_time", entity.LogTime)
        ];
    }

    public QueryParameterCollection ToIdParameterCollection(object id) {
        // id should be a tuple (string batchlogUuid, int id)
        if (id is not ValueTuple<string, int> key)
            throw new ArgumentException("id must be a tuple (string batchlogUuid, int id)");
        return [
            new("@batchlog_uuid", key.Item1),
            new("@id", key.Item2)
        ];
    }
}