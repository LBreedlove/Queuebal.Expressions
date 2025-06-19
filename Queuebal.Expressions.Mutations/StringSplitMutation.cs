using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class StringSplitMutation : Mutation
{
    public static string MutationType => "StringSplit";

    /// <summary>
    /// The separators used to split the string.
    /// </summary>
    public required List<string> Separators { get; set; }

    /// <summary>
    /// If true, empty entries will be removed from the result.
    /// </summary>
    public bool RemoveEmptyEntries { get; set; } = false;

    /// <summary>
    /// If true, leading and trailing whitespace will be removed from each entry in the result.
    /// This is applied after splitting the string.
    /// If RemoveEmptyEntries is true, this will also remove entries that are only whitespace.
    /// </summary>
    public bool TrimEntries { get; set; } = false;

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsString)
        {
            throw new InvalidOperationException("StringSplitMutation can only be applied to a string input value.");
        }

        var splitOptions = StringSplitOptions.None;
        if (RemoveEmptyEntries)
        {
            splitOptions |= StringSplitOptions.RemoveEmptyEntries;
        }

        if (TrimEntries)
        {
            splitOptions |= StringSplitOptions.TrimEntries;
        }

        return inputValue.StringValue
            .Split(Separators.Select(s => s[0]).ToArray(), splitOptions)
            .Select(s => new JSONValue(s))
            .ToList();
    }
}
