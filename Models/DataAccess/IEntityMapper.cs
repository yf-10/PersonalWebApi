namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// Generic interface for mapping a database row (Dictionary) to an entity.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IEntityMapper<T> {
    /// <summary>
    /// Maps a dictionary row to an entity object.
    /// </summary>
    /// <param name="row">The dictionary representing a row from the database.</param>
    /// <returns>An entity object.</returns>
    T MapRowToObject(Dictionary<string, object?> row);
}