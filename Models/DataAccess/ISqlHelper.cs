using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// Generic interface for SQL and parameter generation for repositories.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface ISqlHelper<T> {
    /// <summary>
    /// Gets the SQL statement to select all records.
    /// </summary>
    /// <returns>SQL string for selecting all records.</returns>
    string GetSelectAllSql();

    /// <summary>
    /// Gets the SQL statement to select a record by its ID.
    /// </summary>
    /// <returns>SQL string for selecting a record by ID.</returns>
    string GetSelectByIdSql();

    /// <summary>
    /// Gets the SQL statement to insert a new record.
    /// </summary>
    /// <returns>SQL string for inserting a new record.</returns>
    string GetInsertSql();

    /// <summary>
    /// Gets the SQL statement to update an existing record.
    /// </summary>
    /// <returns>SQL string for updating a record.</returns>
    string GetUpdateSql();

    /// <summary>
    /// Converts an entity to a QueryParameterCollection for SQL operations.
    /// </summary>
    /// <param name="entity">The entity object.</param>
    /// <returns>A QueryParameterCollection with parameters for SQL.</returns>
    QueryParameterCollection ToParameterCollection(T entity);

    /// <summary>
    /// Converts an ID value to a QueryParameterCollection for SQL operations.
    /// </summary>
    /// <param name="id">The ID value.</param>
    /// <returns>A QueryParameterCollection with the ID parameter.</returns>
    QueryParameterCollection ToIdParameterCollection(object id);
}