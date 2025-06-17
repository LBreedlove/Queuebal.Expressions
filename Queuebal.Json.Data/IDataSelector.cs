using Queuebal.Json;

namespace Queuebal.Json.Data;


/// <summary>
/// Represents the result of a data selection operation.
/// </summary>
public class DataSelectorResult
{
    /// <summary>
    /// The found value, if any.
    /// </summary>
    private readonly JSONValue? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSelectorResult"/> class.
    /// </summary>
    /// <param name="path">The path in the JSON data where the value was found.</param>
    /// <param name="value">The value that was found, or an empty JSONValue if not found.</param>
    private DataSelectorResult(string path, JSONValue value, bool isSelectedAsList)
    {
        Found = true;
        Path = path;
        IsSelectedAsList = isSelectedAsList;
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSelectorResult"/> class, to indicate
    /// the value was not found.
    /// </summary>
    private DataSelectorResult(string path)
    {
        Found = false;
        Path = path;
        IsSelectedAsList = false;
    }

    /// <summary>
    /// A DataSelectorResult indicating that the value was found.
    /// </summary>
    /// <param name="path">The path where the value was found.</param>
    /// <param name="value">The located value.</param>
    public static DataSelectorResult Located(string path, JSONValue value, bool isSelectedAsList)
    {
        return new DataSelectorResult(path, value, isSelectedAsList);
    }

    /// <summary>
    /// A DataSelectorResult indicating that the value was not found.
    /// </summary>
    /// <returns>A new DataSelectorResult instance, configured to indicate the value was not found.</returns>
    public static DataSelectorResult NotFound(string path)
    {
        return new DataSelectorResult(path);
    }

    /// <summary>
    /// Indicates if the value was found.
    /// </summary>
    public bool Found { get; }

    /// <summary>
    /// The path in the JSON data where the value was found, or
    /// the path that was searched if the value was not found.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Indicates if the value was selected as a list, or if it was a single value.
    /// </summary>
    /// <remarks>
    /// This property is false if the path was not a list, or when selecting from a list,
    /// and an indexer for an individual item was used (e.g. "path[0]").
    /// </remarks>
    public bool IsSelectedAsList { get; }

    /// <summary>
    /// The value that was found.
    /// </summary>
    /// <raises>
    /// Throws an InvalidOperationException if the value was not found.
    /// </raises>
    public JSONValue Value => Found ? _value! : throw new InvalidOperationException("Value not found.");
}

/// <summary>
/// Defines the interface for a data selector type.
/// </summary>
public interface IDataSelector
{
    /// <summary>
    /// Attempts to find the requested value in the input JSON value.
    /// </summary>
    /// <returns>
    /// A Tuple<bool, JSONValue> where the first item indicates if the configured
    /// path was found in the inputValue, and the second value represents the found item.
    /// </returns>
    IEnumerable<DataSelectorResult> GetValues(JSONValue inputValue);
}
