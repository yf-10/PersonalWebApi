using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// Repository for accessing BatchlogMain data in the database.
/// </summary>
/// <remarks>
/// Constructor for BatchlogMainRepository with custom SQL helper and mapper.
/// </remarks>
/// <param name="worker">The PostgresDbWorker instance for database operations.</param>
/// <param name="sqlHelper">The SQL helper for BatchlogMain.</param>
/// <param name="mapper">The mapper for BatchlogMain.</param>
public class BatchlogMainRepository(PostgresDbWorker worker, ISqlHelper<BatchlogMain> sqlHelper, IEntityMapper<BatchlogMain> mapper) : IRepository<BatchlogMain> {
    private readonly PostgresDbWorker _worker = worker;
    private readonly ISqlHelper<BatchlogMain> _sqlHelper = sqlHelper;
    private readonly IEntityMapper<BatchlogMain> _mapper = mapper;

    /// <summary>
    /// Constructor for BatchlogMainRepository.
    /// </summary>
    /// <param name="worker">The PostgresDbWorker instance for database operations.</param>
    public BatchlogMainRepository(PostgresDbWorker worker)
        : this(worker, new BatchlogMainSqlHelper(), new BatchlogMainMapper()) { }

    /// <summary>
    /// Retrieves all BatchlogMain records from the database.
    /// </summary>
    /// <returns>List of BatchlogMain objects.</returns>
    public List<BatchlogMain> GetAll() {
        var rows = _worker.ExecuteSqlGetList(_sqlHelper.GetSelectAllSql());
        var result = new List<BatchlogMain>();
        foreach (var row in rows) {
            result.Add(_mapper.MapRowToObject(row));
        }
        return result;
    }

    /// <summary>
    /// Asynchronously retrieves all BatchlogMain records from the database.
    /// </summary>
    /// <returns>List of BatchlogMain objects.</returns>
    public async Task<List<BatchlogMain>> GetAllAsync() {
        var rows = await _worker.ExecuteSqlGetListAsync(_sqlHelper.GetSelectAllSql());
        var result = new List<BatchlogMain>();
        foreach (var row in rows) {
            result.Add(_mapper.MapRowToObject(row));
        }
        return result;
    }

    /// <summary>
    /// Retrieves a BatchlogMain record by UUID.
    /// </summary>
    /// <param name="uuid">The UUID of the BatchlogMain record.</param>
    /// <returns>The BatchlogMain object if found; otherwise, null.</returns>
    public BatchlogMain? GetByUuid(string uuid) {
        var prms = _sqlHelper.ToIdParameterCollection(uuid);
        var rows = _worker.ExecuteSqlGetList(_sqlHelper.GetSelectByIdSql(), prms);
        foreach (var row in rows) {
            return _mapper.MapRowToObject(row);
        }
        return null;
    }

    /// <summary>
    /// Asynchronously retrieves a BatchlogMain record by UUID.
    /// </summary>
    /// <param name="uuid">The UUID of the BatchlogMain record.</param>
    /// <returns>The BatchlogMain object if found; otherwise, null.</returns>
    public async Task<BatchlogMain?> GetByUuidAsync(string uuid) {
        var prms = _sqlHelper.ToIdParameterCollection(uuid);
        var rows = await _worker.ExecuteSqlGetListAsync(_sqlHelper.GetSelectByIdSql(), prms);
        foreach (var row in rows) {
            return _mapper.MapRowToObject(row);
        }
        return null;
    }

    /// <summary>
    /// Inserts a new BatchlogMain record into the database.
    /// </summary>
    /// <param name="log">The BatchlogMain object to insert.</param>
    /// <returns>The number of rows affected.</returns>
    public int Insert(BatchlogMain log) {
        return _worker.ExecuteSql(_sqlHelper.GetInsertSql(), _sqlHelper.ToParameterCollection(log));
    }

    /// <summary>
    /// Asynchronously inserts a new BatchlogMain record into the database.
    /// </summary>
    /// <param name="log">The BatchlogMain object to insert.</param>
    /// <returns>The number of rows affected.</returns>
    public Task<int> InsertAsync(BatchlogMain log) {
        return _worker.ExecuteSqlAsync(_sqlHelper.GetInsertSql(), _sqlHelper.ToParameterCollection(log));
    }

    /// <summary>
    /// Updates an existing BatchlogMain record in the database.
    /// </summary>
    /// <param name="log">The BatchlogMain object to update.</param>
    /// <returns>The number of rows affected.</returns>
    public int Update(BatchlogMain log) {
        return _worker.ExecuteSql(_sqlHelper.GetUpdateSql(), _sqlHelper.ToParameterCollection(log));
    }

    /// <summary>
    /// Asynchronously updates an existing BatchlogMain record in the database.
    /// </summary>
    /// <param name="log">The BatchlogMain object to update.</param>
    /// <returns>The number of rows affected.</returns>
    public Task<int> UpdateAsync(BatchlogMain log) {
        return _worker.ExecuteSqlAsync(_sqlHelper.GetUpdateSql(), _sqlHelper.ToParameterCollection(log));
    }

    /// <summary>
    /// Inserts multiple BatchlogMain records into the database.
    /// </summary>
    /// <param name="logs">The collection of BatchlogMain objects to insert.</param>
    /// <returns>The total number of rows affected.</returns>
    public int InsertAll(IEnumerable<BatchlogMain> logs) {
        int count = 0;
        foreach (var log in logs) {
            count += Insert(log);
        }
        return count;
    }

    /// <summary>
    /// Asynchronously inserts multiple BatchlogMain records into the database.
    /// </summary>
    /// <param name="logs">The collection of BatchlogMain objects to insert.</param>
    /// <returns>The total number of rows affected.</returns>
    public async Task<int> InsertAllAsync(IEnumerable<BatchlogMain> logs) {
        int count = 0;
        foreach (var log in logs) {
            count += await InsertAsync(log);
        }
        return count;
    }
}