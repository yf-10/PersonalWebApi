using System.Data;
using System.Collections;

namespace PersonalWebApi.Utilities;

/// <summary>
/// Represents a collection of QueryParameter objects for SQL commands.
/// </summary>
public class QueryParameterCollection : IEnumerable<QueryParameter> {
    // Internal list to store parameters
    private readonly List<QueryParameter> _parameters = [];

    /// <summary>
    /// Initializes a new empty QueryParameterCollection.
    /// </summary>
    public QueryParameterCollection() { }

    /// <summary>
    /// Initializes a new QueryParameterCollection with the specified parameters.
    /// </summary>
    /// <param name="parameters">The initial set of parameters.</param>
    public QueryParameterCollection(IEnumerable<QueryParameter> parameters) {
        _parameters.AddRange(parameters);
    }

    /// <summary>
    /// Adds a QueryParameter to the collection.
    /// </summary>
    /// <param name="parameter">The parameter to add.</param>
    public void Add(QueryParameter parameter) => _parameters.Add(parameter);

    /// <summary>
    /// Gets the number of parameters in the collection.
    /// </summary>
    public int Count => _parameters.Count;

    /// <summary>
    /// Gets the parameter at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the parameter to get.</param>
    public QueryParameter this[int index] => _parameters[index];

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    public IEnumerator<QueryParameter> GetEnumerator() => _parameters.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection (non-generic).
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => _parameters.GetEnumerator();
}

/// <summary>
/// Represents a parameter for SQL commands (DBMS independent).
/// </summary>
public class QueryParameter {
    /// <summary>
    /// Parameter name (e.g., "@id").
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Parameter value.
    /// </summary>
    public object? Value { get; }
    /// <summary>
    /// Parameter data type (ADO.NET standard).
    /// </summary>
    public DbType Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameter"/> class.
    /// </summary>
    /// <param name="name">Parameter name (e.g., "@id").</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="type">Parameter data type.</param>
    public QueryParameter(string name, object? value, DbType type) {
        Name = name;
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameter"/> class (type inferred from value).
    /// </summary>
    /// <param name="name">Parameter name (e.g., "@id").</param>
    /// <param name="value">Parameter value.</param>
    public QueryParameter(string name, object? value) {
        Name = name;
        Value = value;
        Type = value switch {
            string => DbType.String,
            int => DbType.Int32,
            long => DbType.Int64,
            bool => DbType.Boolean,
            decimal => DbType.Decimal,
            double => DbType.Double,
            float => DbType.Single,
            DateTime => DbType.DateTime,
            null => DbType.Object,
            _ => DbType.Object
        };
    }
}