namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// Generic repository interface for basic CRUD operations.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IRepository<T> {
    /// <summary>
    /// Gets all records from the database.
    /// </summary>
    /// <returns>A list of entities.</returns>
    List<T> GetAll();

    /// <summary>
    /// Gets all records from the database asynchronously.
    /// </summary>
    /// <returns>A list of entities.</returns>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Inserts a new record into the database.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <returns>The number of rows affected.</returns>
    int Insert(T entity);

    /// <summary>
    /// Inserts a new record into the database asynchronously.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> InsertAsync(T entity);
}