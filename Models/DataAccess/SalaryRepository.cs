using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// Repository for accessing Salary data in the database.
/// </summary>
public class SalaryRepository : IRepository<Salary> {
    private readonly PostgresDbWorker _worker;
    private readonly ISqlHelper<Salary> _sqlHelper;
    private readonly IEntityMapper<Salary> _mapper;

    /// <summary>
    /// Constructor for SalaryRepository.
    /// </summary>
    /// <param name="worker">The PostgresDbWorker instance for database operations.</param>
    public SalaryRepository(PostgresDbWorker worker)
        : this(worker, new SalarySqlHelper(), new SalaryMapper()) { }

    /// <summary>
    /// Constructor for SalaryRepository with custom SQL helper and mapper.
    /// </summary>
    public SalaryRepository(PostgresDbWorker worker, ISqlHelper<Salary> sqlHelper, IEntityMapper<Salary> mapper) {
        _worker = worker;
        _sqlHelper = sqlHelper;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all Salary records from the database.
    /// </summary>
    public List<Salary> GetAll() {
        var rows = _worker.ExecuteSqlGetList(_sqlHelper.GetSelectAllSql());
        var result = new List<Salary>();
        foreach (var row in rows) {
            result.Add(_mapper.MapRowToObject(row));
        }
        return result;
    }

    /// <summary>
    /// Gets all Salary records from the database asynchronously.
    /// </summary>
    public async Task<List<Salary>> GetAllAsync() {
        var rows = await _worker.ExecuteSqlGetListAsync(_sqlHelper.GetSelectAllSql());
        var result = new List<Salary>();
        foreach (var row in rows) {
            result.Add(_mapper.MapRowToObject(row));
        }
        return result;
    }

    /// <summary>
    /// Inserts a new Salary record into the database.
    /// </summary>
    public int Insert(Salary entity) {
        return _worker.ExecuteSql(_sqlHelper.GetInsertSql(), _sqlHelper.ToParameterCollection(entity));
    }

    /// <summary>
    /// Inserts a new Salary record into the database asynchronously.
    /// </summary>
    public Task<int> InsertAsync(Salary entity) {
        return _worker.ExecuteSqlAsync(_sqlHelper.GetInsertSql(), _sqlHelper.ToParameterCollection(entity));
    }

    /// <summary>
    /// Inserts multiple Salary records into the database.
    /// </summary>
    /// <param name="entities">The collection of Salary objects to insert.</param>
    /// <returns>The total number of rows affected.</returns>
    public int InsertAll(IEnumerable<Salary> entities) {
        int count = 0;
        foreach (var entity in entities) {
            count += Insert(entity);
        }
        return count;
    }

    /// <summary>
    /// Inserts multiple Salary records into the database asynchronously.
    /// </summary>
    /// <param name="entities">The collection of Salary objects to insert.</param>
    /// <returns>The total number of rows affected.</returns>
    public async Task<int> InsertAllAsync(IEnumerable<Salary> entities) {
        int count = 0;
        foreach (var entity in entities) {
            count += await InsertAsync(entity);
        }
        return count;
    }
}