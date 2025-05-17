using System.Data;

namespace PersonalWebApi.Utilities;

/// <summary>
/// Represents a parameter for SQL commands.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SqlParameter"/> class.
/// </remarks>
public class SqlParameter(string name, object value, DbType type) {
    /// <summary>Parameter name.</summary>
    public string Name { get; } = name;
    /// <summary>Parameter value.</summary>
    public object Value { get; } = value;
    /// <summary>Parameter data type.</summary>
    public DbType Type { get; } = type;
}