namespace Queuebal.Json.Data;

/// <summary>
/// A class for segmenting a JSON path into its components.
/// </summary>
/// <remarks>
/// The path should consist of segments separated by dots (.) and may include array indices in square brackets ([]).
/// For example, "data.items[0].name" would be segmented into ["data", "items", "[0]", "name"].
/// You can also specify a range of indices using the format "[start:end]", where 'start' and 'end' are optional.
/// For example, "data.items[1:3]" would be segmented into ["data", "items", "[1:3]"]. "[1:3]" will capture items 1 and 2
/// in the list.
public static class PathSegmenter
{
    /// <summary>
    /// Gets the segments of the search path.
    /// </summary>
    /// <returns>An enumerable of search path segments.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when an invalid path character is encountered, given the parser's current state.
    /// </exception>
    public static IEnumerable<string> GetPathSegments(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            yield break;
        }

        int index = 0;
        int previousSegmentEnd = 0;
        bool searchingForEndArrayIndex = false;
        bool previousSegmentWasArrayIndex = false;
        bool searchingForSeparator = false;
        bool expectingSeparator = false;

        foreach (var c in path)
        {
            if (c == '.')
            {
                if (!searchingForSeparator)
                {
                    throw new ArgumentOutOfRangeException(nameof(path), "Unexpected dot character in path while not searching for a token.");
                }

                if (searchingForEndArrayIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(path), "Unexpected dot character in path while searching for end of array index.");
                }

                if (previousSegmentWasArrayIndex)
                {
                    // skip the dot char after the previous array index ended.
                    previousSegmentWasArrayIndex = false;
                    previousSegmentEnd = index + 1;
                    expectingSeparator = false;
                    searchingForSeparator = false;
                    ++index;
                    continue;
                }

                // yield the previous segment
                var segment = path.Substring(previousSegmentEnd, index - previousSegmentEnd);
                yield return segment;

                // skip the dot char
                previousSegmentEnd = index + 1;
                searchingForSeparator = false;
                expectingSeparator = false;
            }
            else if (c == '[')
            {
                if (searchingForEndArrayIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(path), "Unexpected opening bracket in path while searching for end of array index.");
                }

                searchingForSeparator = false;
                expectingSeparator = false;
                if (previousSegmentWasArrayIndex)
                {
                    previousSegmentWasArrayIndex = false;
                    searchingForEndArrayIndex = true;
                    previousSegmentEnd = index;
                    ++index;
                    continue;
                }

                if (previousSegmentEnd != index)
                {
                    var segment = path.Substring(previousSegmentEnd, index - previousSegmentEnd);
                    yield return segment;
                }

                // don't skip the opening bracket
                previousSegmentEnd = index;
                searchingForEndArrayIndex = true;
            }
            else if (c == ']')
            {
                if (!searchingForEndArrayIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(path), "Unexpected closing bracket in path while not searching for end of array index.");
                }

                // yield the array index segment
                var segment = path.Substring(previousSegmentEnd, index - previousSegmentEnd + 1);
                yield return segment;

                // reset the previous segment end to the current index
                previousSegmentEnd = index;
                previousSegmentWasArrayIndex = true;
                searchingForEndArrayIndex = false;
                searchingForSeparator = true;
                expectingSeparator = true;
            }
            else if (expectingSeparator)
            {
                throw new ArgumentOutOfRangeException(nameof(path), $"Unexpected character '{c}' in path while expecting a separator.");
            }
            else
            {
                searchingForSeparator = true;
            }
            ++index;
        }

        // TODO: The code below is pretty hacky, but it works for now.
        if (previousSegmentEnd >= path.Length - 1)
        {
            // If the last character is a dot, we should not yield an empty segment.
            if (path[path.Length - 1] == '.')
            {
                throw new ArgumentOutOfRangeException(nameof(path), "Path ends with a dot without a following segment.");
            }
        }

        if (previousSegmentEnd <= path.Length - 1)
        {
            if (searchingForEndArrayIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(path), "Path ends with an opening bracket without a closing bracket.");
            }

            if (previousSegmentWasArrayIndex)
            {
                yield break;
            }

            var segment = path.Substring(previousSegmentEnd, index - previousSegmentEnd);
            yield return segment;
        }
    }
}
