using System.Text;

namespace Queuebal.Json.Data;


/// <summary>
/// Writes data to a JSON structure based on specified paths and values.
/// </summary>
public class DataWriter : IDataWriter
{

    /// <summary>
    /// Writes the specified value to the output JSON value at the specified path.
    /// </summary>
    public JSONValue WriteValue(string path, JSONValue value)
    {
        return WriteValues
        (
            new[]
            {
                new DataWriterValuePath { Path = path, Value = value  }
            }
        );
    }

    /// <summary>
    /// Writes the specified values to the output JSON value at their respective paths.
    /// </summary>
    /// <param name="values">The value/paths to write to a new JSONObject.</param>
    /// <returns>The constructed JSONValue object.</returns>
    public JSONValue WriteValues(IEnumerable<DataWriterValuePath> values)
    {
        JSONValue? outputValue = null;
        foreach (var valuePath in values)
        {
            var pathSegments = PathSegmenter.GetPathSegments(valuePath.Path);
            outputValue = WriteValue(ref outputValue, pathSegments, valuePath.Value, new StringBuilder());
        }
        return outputValue ?? new JSONValue();
    }

    /// <summary>
    /// Writes a value to the output JSONValue at the specified path.
    /// </summary>
    /// <param name="outputValue">The outputValue to write to. If null, a new JSONValue object will be created.</param>
    /// <param name="path">The path to write the value to.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The resulting outputValue JSONValue object.</returns>
    public JSONValue WriteValue(ref JSONValue? outputValue, string path, JSONValue value)
    {
        var pathSegments = PathSegmenter.GetPathSegments(path);
        return WriteValue(ref outputValue, pathSegments, value, new StringBuilder());
    }

    // TODO: Refactor this method to be more readable and maintainable
    // NOTE: When refactoring, we don't want to use recursion here, as it can lead to stack overflow for deep paths.
    private JSONValue WriteValue(ref JSONValue? outputValue, IEnumerable<string> pathSegments, JSONValue value, StringBuilder currentPath)
    {
        if (outputValue == null)
        {
            if (!pathSegments.Any())
            {
                outputValue = value.Clone();
                return outputValue;
            }

            if (IsListAccessorSegment(pathSegments.First()))
            {
                // if the path is a list segment, we need to create a new list
                outputValue = new JSONValue(new List<JSONValue>());
            }
            else
            {
                outputValue = new JSONValue(new Dictionary<string, JSONValue>());
            }
        }
        else if (!pathSegments.Any())
        {
            // if the path is empty, we just return the value
            if (outputValue.IsObject || outputValue.IsList)
            {
                // if the output value is an object or a list, we cannot write to an empty path
                throw new InvalidOperationException("Cannot write to an empty path when an object or list already exists.");
            }

            outputValue = value.Clone();
            return outputValue;
        }

        int index = 0;
        bool isFirstSegment = true;

        var currentNode = outputValue;
        foreach (var segment in pathSegments)
        {
            if (segment.StartsWith("["))
            {
                currentPath.Append(segment);

                // this is an array index segment
                if (!currentNode.IsList)
                {
                    // tried to write into a non-list - throw an exception
                    throw new InvalidOperationException($"Cannot write to path '{currentPath}' because the current node is not a list.");
                }

                WriteListValue(currentNode, pathSegments.Skip(index), currentPath, value);
                return outputValue;
            }

            if (isFirstSegment)
            {
                currentPath.Append(segment);
                isFirstSegment = false;
            }
            else
            {
                currentPath.Append("." + segment);
            }

            // this is an object property segment
            if (!currentNode.IsObject)
            {
                // attempted to access a property on a non-object - return NotFound
                throw new InvalidOperationException($"Cannot write to path '{segment}' because the current node is not an object.");
            }

            currentNode.DictValue.TryGetValue(segment, out var nextNode);
            if (nextNode == null)
            {
                // the segment was not found in the current node - build the rest of the path
                if (IsLeafValue(pathSegments.Skip(index)))
                {
                    // if the next segment is a leaf value, we can write the value directly
                    currentNode.DictValue[segment] = value;
                    return outputValue;
                }

                if (IsListSegment(pathSegments.Skip(index)))
                {
                    // if the next segment is a list, we need to create a new list node
                    nextNode = new JSONValue(new List<JSONValue>());
                    currentNode.DictValue[segment] = nextNode;
                    currentNode = nextNode;
                }
                else
                {
                    // otherwise, we need to create a new node for this segment
                    nextNode = new JSONValue(new Dictionary<string, JSONValue>());
                    currentNode.DictValue[segment] = nextNode;
                    currentNode = nextNode;
                }
            }
            else
            {
                // the segment was found in the current node - continue down the path
                currentNode = nextNode;
            }

            ++index;
        }

        return outputValue;
    }

    // TODO: Refactor this method to be more readable and maintainable
    private JSONValue WriteListValue(JSONValue currentNode, IEnumerable<string> segmentsRemaining, StringBuilder currentPath, JSONValue value)
    {
        // currentNode should be a JSONValue containing a list
        // the first segment should be the list accessor segment (e.g. "[0]")
        var indexRange = IndexRange.Build(segmentsRemaining.First());
        if (indexRange.IsSingleItem)
        {
            // If it's a single item, we can write the value at that index
            int writeTo = indexRange.Start;
            if (writeTo >= currentNode.ListValue.Count)
            {
                // If the index is out of bounds, we need to expand the list
                for (int index = currentNode.ListValue.Count; index <= writeTo; ++index)
                {
                    currentNode.ListValue.Add(new JSONValue()); // Add empty values until we reach the desired index
                }
            }

            // if this segment is a leaf, we can write the value directly
            if (IsLeafValue(segmentsRemaining))
            {
                // if the next segment is a leaf value, we can write the value directly
                currentNode.ListValue[writeTo] = value;
                return currentNode;
            }
            else if (IsListAccessorSegment(segmentsRemaining.Skip(1).First()))
            {
                // if the next segment is a list accessor, we need to create a new list node
                var newNode = new JSONValue(new List<JSONValue>());
                currentNode.ListValue[writeTo] = newNode;
                currentNode = newNode;
            }
            else
            {
                var newNode = new JSONValue(new Dictionary<string, JSONValue>());
                currentNode.ListValue[writeTo] = newNode;
                currentNode = newNode;
            }

            return WriteValue(ref currentNode!, segmentsRemaining.Skip(1), value, currentPath);
        }

        // otherwise, we need to create a new JSONValue at the end
        if (IsLeafValue(segmentsRemaining))
        {
            // if the next segment is a leaf value, we can write the value directly
            currentNode.ListValue.Add(value);
            return currentNode;
        }
        else if (IsListAccessorSegment(segmentsRemaining.Skip(1).First()))
        {
            // if the next segment is a list accessor, we need to create a new list node
            var newNode = new JSONValue(new List<JSONValue>());
            currentNode.ListValue.Add(newNode);
            currentNode = newNode;
        }
        else
        {
            // otherwise, we need to create a new node for this segment
            var newNode = new JSONValue(new Dictionary<string, JSONValue>());
            currentNode.ListValue.Add(newNode);
            currentNode = newNode;
        }

        return WriteValue(ref currentNode!, segmentsRemaining.Skip(1), value, currentPath);
    }

    /// <summary>
    /// Indicates if the pathSegments represent a leaf value.
    /// </summary>
    /// <param name="pathSegments">The current path segments to check.</param>
    /// <returns>True if there are no more segments after the current segment, otherwise false.</returns>
    private static bool IsLeafValue(IEnumerable<string> pathSegments)
    {
        // A leaf value is defined as a path that has no segments left after the current one
        return !pathSegments.Skip(1).Any();
    }

    /// <summary>
    /// Indicates whether the next segment in the path is a list segment.
    /// </summary>
    /// <param name="segmentsRemaining">The segments remaining after the current segment.</param>
    /// <returns>true if the next segment references a list object, otherwise false.</returns>
    /// <remarks
    /// This method skips the first two segments to see if a list accesor is attached to the next segment.
    /// e.g. 'current_segment.list_segment[0]'; we skip 'current_segment' because it's the current segment,
    /// and then see if a list accessor is attached to the next segment (list_segment).
    /// </remarks>
    private static bool IsListSegment(IEnumerable<string> segmentsRemaining)
    {
        var nextSegment = segmentsRemaining.Skip(1).FirstOrDefault();
        if (nextSegment == null)
        {
            // we shouldn't hit this because the caller checks to see if the node
            // IsLeafValue first, which would return true in this case.
            return false; // No next segment, so not a list
        }

        return IsListAccessorSegment(nextSegment);
    }

    /// <summary>
    /// Indicates whether the segment is a list accessor segment.
    /// A list accessor segment is defined as a segment that starts with "[" and ends with "]".
    /// </summary>
    /// <param name="segment">The segment to check.</param>
    /// <returns>true if the segment appears to be a list accessor, otherwise false.</returns>
    private static bool IsListAccessorSegment(string segment)
    {
        return segment.StartsWith("[") && segment.EndsWith("]");
    }
}
