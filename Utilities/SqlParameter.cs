using System.Data;

namespace PersonalWebApi.Utilities;

/// <summary>
/// Represents a parameter for SQL commands (DBMS independent).
/// </summary>
public class SqlParameter
{
    /// <summary>Parameter name (e.g., "@id").</summary>
    public string Name { get; }
    /// <summary>Parameter value.</summary>
    public object? Value { get; }
    /// <summary>Parameter data type (ADO.NET standard).</summary>
    public DbType Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlParameter"/> class.
    /// </summary>
    /// <param name="name">Parameter name (e.g., "@id").</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="type">Parameter data type.</param>
    public SqlParameter(string name, object? value, DbType type)
    {
        Name = name;
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlParameter"/> class (type inferred from value).
    /// </summary>
    /// <param name="name">Parameter name (e.g., "@id").</param>
    /// <param name="value">Parameter value.</param>
    public SqlParameter(string name, object? value)
    {
        Name = name;
        Value = value;
        Type = value switch
        {
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