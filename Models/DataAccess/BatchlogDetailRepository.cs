using PersonalWebApi.Models.Data;
using PersonalWebApi.Utilities;

namespace PersonalWebApi.Models.DataAccess;

/// <summary>
/// Repository for accessing BatchlogDetail data in the database.
/// </summary>
public class BatchlogDetailRepository : IRepository<BatchlogDetail> {
    private readonly PostgresDbWorker _worker;
    private readonly ISqlHelper<BatchlogDetail> _sqlHelper;
    private readonly IEntityMapper<BatchlogDetail> _mapper;

    /// <summary>
    /// Constructor for BatchlogDetailRepository.
    /// </summary>
    public BatchlogDetailRepository(PostgresDbWorker worker)
        : this(worker, new BatchlogDetailSqlHelper(), new BatchlogDetailMapper()) { }

    /// <summary>
    /// Constructor for BatchlogDetailRepository with custom SQL helper and mapper.
    /// </summary>
    public BatchlogDetailRepository(PostgresDbWorker worker, ISqlHelper<BatchlogDetail> sqlHelper, IEntityMapper<BatchlogDetail> mapper) {
        _worker = worker;
        _sqlHelper = sqlHelper;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all BatchlogDetail records from the database.
    /// </summary>
    public List<BatchlogDetail> GetAll() {
        var rows = _worker.ExecuteSqlGetList(_sqlHelper.GetSelectAllSql());
        var result = new List<BatchlogDetail>();
        foreach (var row in rows) {
            result.Add(_mapper.MapRowToObject(row));
        }
        return result;
    }

    /// <summary>
    /// Gets all BatchlogDetail records from the database asynchronously.
    /// </summary>
    public async Task<List<BatchlogDetail>> GetAllAsync() {
        var rows = await _worker.ExecuteSqlGetListAsync(_sqlHelper.GetSelectAllSql());
        var result = new List<BatchlogDetail>();
        foreach (var row in rows) {
            result.Add(_mapper.MapRowToObject(row));
        }
        return result;
    }

    /// <summary>
    /// Inserts a new BatchlogDetail record into the database.
    /// </summary>
    public int Insert(BatchlogDetail entity) {
        return _worker.ExecuteSql(_sqlHelper.GetInsertSql(), _sqlHelper.ToParameterCollection(entity));
    }

    /// <summary>
    /// Inserts a new BatchlogDetail record into the database asynchronously.
    /// </summary>
    public Task<int> InsertAsync(BatchlogDetail entity) {
        return _worker.ExecuteSqlAsync(_sqlHelper.GetInsertSql(), _sqlHelper.ToParameterCollection(entity));
    }
}