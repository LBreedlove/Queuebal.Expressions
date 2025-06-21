using System.Text;

namespace Queuebal.Json.Data;


/// <summary>
/// Provides access to data within a JSON structure based on a specified path.
/// </summary>
public class DataSelector : IDataSelector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataSelector"/> class.
    /// </summary>
    /// <param name="dataSelectorConfig">The configuration for the data selector.</param>
    public DataSelector(string path)
    {
        Path = path;
    }

    /// <summary>
    /// The path of the value to select.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Attempts to get the value from the input JSON value, based on the DataSelector's Path.
    /// </summary>
    /// <param name="inputValue">The input JSON value to search in.</param>
    /// <returns>
    /// A Tuple where the first item indicates if the value was found, and the second item is the found JSONValue.
    /// If the value was not found, the first item will be false and the second item will be an empty JSONValue.
    /// </returns>
    public IEnumerable<DataSelectorResult> GetValues(JSONValue inputValue)
    {
        if (inputValue.IsNull)
        {
            yield return DataSelectorResult.NotFound(path: "");
            yield break;
        }

        var segments = PathSegmenter.GetPathSegments(Path);
        foreach (var value in GetValues(inputValue, segments, new StringBuilder()))
        {
            yield return value;
        }
    }

    /// <summary>
    /// Attempts to get the value from the input JSON value, based on the provided path segments.
    /// </summary>
    /// <param name="inputValue">The input JSON value to search in.</param>
    /// <param name="segments">The segments of the path to search for.</param>
    /// <returns>
    /// A Tuple where the first item indicates if the value was found, and the second item is the found JSONValue.
    /// If the value was not found, the first item will be false and the second item will be an empty JSONValue.
    /// </returns>
    private IEnumerable<DataSelectorResult> GetValues(JSONValue inputValue, IEnumerable<string> segments, StringBuilder currentPath, bool selectedAsList = false)
    {
        var currentReadNode = inputValue;

        // we don't check the inputValue.IsNull here, because we already checked in the caller
    
        bool isFirstSegment = currentPath.Length == 0;
        int index = 0;
        foreach (var segment in segments)
        {
            if (segment.StartsWith("["))
            {
                // this is an array index segment
                if (!currentReadNode.IsList)
                {
                    currentPath.Append(segment);

                    // tried to index into a non-list - return NotFound
                    yield return DataSelectorResult.NotFound(path: currentPath.ToString());
                    yield break;
                }

                var segmentsRemaining = segments.Skip(index);

                // we don't update currentPath here, because it will be updated by GetListItems
                foreach (var result in GetListItems(currentReadNode, segmentsRemaining, currentPath, selectedAsList))
                {
                    yield return result;
                }
                yield break;
            }
            else if (!currentReadNode.IsDict)
            {
                if (isFirstSegment)
                {
                    currentPath.Append(segment);
                }
                else
                {
                    currentPath.Append("." + segment);
                }

                // attempted to access a property on a non-object - return NotFound
                yield return DataSelectorResult.NotFound(path: currentPath.ToString());
                yield break;
            }

            if (isFirstSegment)
            {
                currentPath.Append(segment);
            }
            else
            {
                currentPath.Append("." + segment);
            }

            currentReadNode.DictValue.TryGetValue(segment, out var nextNode);
            if (nextNode == null)
            {
                // the segment was not found in the current node - return NotFound
                yield return DataSelectorResult.NotFound(path: currentPath.ToString());
                yield break;
            }

            isFirstSegment = false;
            currentReadNode = nextNode;
            ++index;
        }

        yield return DataSelectorResult.Located(path: currentPath.ToString(), value: currentReadNode, isSelectedAsList: selectedAsList);
        yield break;
    }

    /// <summary>
    /// Gets the value of the list items.
    /// </summary>
    /// <param name="currentNode">The list node to evaluate.</param>
    /// <param name="segments">The segments remaining in the Path.</param>
    /// <returns>A JSONValue with a list value.</returns>
    private IEnumerable<DataSelectorResult> GetListItems(JSONValue currentNode, IEnumerable<string> segments, StringBuilder currentPath, bool selectingFromList)
    {
        var segment = segments.First();
        bool foundStart = false;

        var originalPath = currentPath.ToString();

        var indexRange = IndexRange.Build(segment);
        for (int index = 0; index < currentNode.ListValue.Count; ++index)
        {
            if (indexRange.ContainsIndex(index))
            {
                foundStart = true;
                var results = GetValues
                (
                    currentNode.ListValue[index],
                    segments.Skip(1),
                    new StringBuilder(originalPath + $"[{index}]"),
                    selectedAsList: selectingFromList || !indexRange.IsSingleItem
                );

                foreach (var result in results)
                {
                    if (result.Found)
                    {
                        yield return result;
                    }
                }
            }
            else if (foundStart)
            {
                // we have found the start of the range, and we're at the end, so we can stop
                break;
            }
        }
    }
}
