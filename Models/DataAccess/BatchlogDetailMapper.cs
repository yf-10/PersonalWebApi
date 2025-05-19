using System.Collections.Generic;
using PersonalWebApi.Models.Data;

namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// Mapper class for converting a database row to a BatchlogDetail entity.
/// Implements IEntityMapper for BatchlogDetail.
/// </summary>
public class BatchlogDetailMapper : IEntityMapper<BatchlogDetail> {
    public BatchlogDetail MapRowToObject(Dictionary<string, object?> row) {
        return new BatchlogDetail(
            row["batchlog_uuid"] as string ?? throw new InvalidCastException("batchlog_uuid is null"),
            row["id"] is int i ? i : Convert.ToInt32(row["id"]),
            row["message"] as string ?? throw new InvalidCastException("message is null"),
            row["log_time"] as DateTime? ?? throw new InvalidCastException("log_time is null"),
            "TEST"
        );
    }
}