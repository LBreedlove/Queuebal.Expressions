namespace Queuebal.Json.Data;

/// <summary>
/// Represents a range of indices in an array or list.
/// </summary>
public class IndexRange
{
    /// <summary>
    /// Builds an IndexRange from a string representation of an index range.
    /// </summary>
    /// <param name="indexRange">The string representing the index range.</param>
    /// <returns></returns>
    public static IndexRange Build(string indexRange)
    {
        if (string.IsNullOrWhiteSpace(indexRange))
        {
            throw new ArgumentException("Index range cannot be null or empty.", nameof(indexRange));
        }

        var indexRangeTrimmed = indexRange.Trim('[', ']').Trim();
        if (string.IsNullOrWhiteSpace(indexRangeTrimmed))
        {
            return new IndexRange { Start = -1, End = -1 }; // If no range is specified, return a range that includes all indices.
        }

        int separatorIndex = indexRangeTrimmed.IndexOf(':');
        if (separatorIndex == -1)
        {
            // If no separator is found, treat the entire string as a single index.
            if (!int.TryParse(indexRangeTrimmed, out int singleIndex))
            {
                throw new ArgumentException($"Invalid index range '{indexRange}'.", nameof(indexRange));
            }

            // Single index is treated as a range [index, index + 1).
            return new IndexRange { Start = singleIndex, End = singleIndex + 1 };
        }

        var startStr = indexRangeTrimmed.Substring(0, separatorIndex).Trim();
        var endStr = indexRangeTrimmed.Substring(separatorIndex + 1).Trim();

        if (startStr == "")
        {
            startStr = "-1"; // If no start index is specified, default to -1.
        }

        if (endStr == "")
        {
            endStr = "-1"; // If no end index is specified, default to -1.
        }

        if (!int.TryParse(startStr, out int start))
        {
            throw new ArgumentException($"Invalid start index '{startStr}' in index range '{indexRange}'.", nameof(indexRange));
        }

        if (!int.TryParse(endStr, out int end))
        {
            throw new ArgumentException($"Invalid end index '{endStr}' in index range '{indexRange}'.", nameof(indexRange));
        }

        return new IndexRange { Start = start, End = end };
    }

    /// <summary>
    /// Indicates if the index range represents a single item.
    /// </summary>
    public bool IsSingleItem => Start == End - 1;

    /// <summary>
    /// Indicates if the index range is empty.
    /// </summary>
    public bool IsEmpty => (Start == End) && (Start != -1);

    /// <summary>
    /// Indicates if the range goes on forever.
    /// </summary>
    public bool IsInfinite => End == -1;

    /// <summary>
    /// The first index in the range, inclusive.
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// The last index in the range, exclusive.
    /// </summary>
    public int End { get; set; }

    /// <summary>
    /// Determines if the index is within the range defined by Start and End.
    /// </summary>
    /// <param name="index">The index to check for inclusion in the range.</param>
    /// <returns>true if the index is within the range, otherwise false.</returns>
    public bool ContainsIndex(int index)
    {
        if (Start == -1)
        {
            if (End == -1)
            {
                return true; // If both Start and End are -1, all indices are included.
            }
            return index < End; // If only Start is -1, check if index is less than End.
        }

        if (End == -1)
        {
            // if end is -1, we include all indices starting from Start
            return index >= Start;
        }

        return index >= Start && index < End;
    }
}
