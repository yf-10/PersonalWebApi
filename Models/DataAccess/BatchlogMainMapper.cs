using PersonalWebApi.Models.Data;

namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// Mapper class for converting a database row to a BatchlogMain entity.
/// Implements IEntityMapper for BatchlogMain.
/// </summary>
public class BatchlogMainMapper : IEntityMapper<BatchlogMain> {
    /// <summary>
    /// Maps a dictionary row to a BatchlogMain object.
    /// </summary>
    /// <param name="row">The dictionary representing a row from the database.</param>
    /// <returns>A BatchlogMain object.</returns>
    /// <exception cref="InvalidCastException">Thrown if required fields are missing or null.</exception>
    public BatchlogMain MapRowToObject(Dictionary<string, object?> row) {
        return new BatchlogMain(
            row["uuid"] as string ?? throw new InvalidCastException("uuid is null"),
            row["status"] as string ?? throw new InvalidCastException("status is null"),
            row["program_id"] as string ?? throw new InvalidCastException("program_id is null"),
            row["program_name"] as string ?? throw new InvalidCastException("program_name is null"),
            row["start_time"] as DateTime?,
            row["end_time"] as DateTime?,
            [] // Details are not loaded in this repository
        );
    }
}