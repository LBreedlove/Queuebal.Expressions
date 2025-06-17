namespace Queuebal.Json.Data;


public class DataWriterValuePath
{
    /// <summary>
    /// The path to write the value to, in the format "field1.field2[0].field3".
    /// </summary>
    public required string Path { get; set; }

    /// <summary>
    /// The value to write at the specified path.
    /// </summary>
    public required JSONValue Value { get; set; }
}


/// <summary>
/// Defines the interface for a data writer type.
/// </summary>
public interface IDataWriter
{
    /// <summary>
    /// Writes the specified value to the output JSON value at the specified path.
    /// </summary>
    JSONValue WriteValue(string path, JSONValue value);

    /// <summary>
    /// Writes the specified values to the output JSON value at their respective paths.
    /// </summary>
    /// <param name="values">The value/paths to write to a new JSONObject.</param>
    /// <returns>The constructed JSONValue object.</returns>
    JSONValue WriteValues(IEnumerable<DataWriterValuePath> values);
}
