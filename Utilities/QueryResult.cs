using System.Collections;
using System.Collections.Generic;

namespace PersonalWebApi.Utilities;

/// <summary>
/// Represents a collection of query result rows, where each row is a dictionary of column name and value.
/// </summary>
public class QueryResult : IEnumerable<Dictionary<string, object?>> {
    // Internal list to store rows
    private readonly List<Dictionary<string, object?>> _rows = [];

    /// <summary>
    /// Initializes a new empty QueryResult.
    /// </summary>
    public QueryResult() { }

    /// <summary>
    /// Initializes a new QueryResult with the specified rows.
    /// </summary>
    /// <param name="rows">The initial set of rows.</param>
    public QueryResult(IEnumerable<Dictionary<string, object?>> rows) {
        _rows.AddRange(rows);
    }

    /// <summary>
    /// Adds a row to the result set.
    /// </summary>
    /// <param name="row">The row to add.</param>
    public void Add(Dictionary<string, object?> row) => _rows.Add(row);

    /// <summary>
    /// Gets the number of rows in the result set.
    /// </summary>
    public int Count => _rows.Count;

    /// <summary>
    /// Gets the row at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the row to get.</param>
    public Dictionary<string, object?> this[int index] => _rows[index];

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    public IEnumerator<Dictionary<string, object?>> GetEnumerator() => _rows.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection (non-generic).
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => _rows.GetEnumerator();
}