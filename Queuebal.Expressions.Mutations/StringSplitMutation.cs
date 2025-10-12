using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class StringSplitMutation : Mutation
{
    public static string MutationType => "StringSplit";

    /// <summary>
    /// The separators used to split the string.
    /// </summary>
    public required List<IExpression> Separators { get; set; }

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

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue originalValue, JSONValue sourceData)
    {
        if (!originalValue.IsString)
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

        var separators = Separators.Select(s => s.Evaluate(context, sourceData)).ToList();
        if (separators.Count == 0)
        {
            throw new InvalidOperationException("StringSplitMutation requires at least one separator.");
        }

        if (separators.Select(s => s.IsString).Any(v => !v))
        {
            throw new InvalidOperationException("All Separators expressions must evaluate to string values.");
        }

        return originalValue.StringValue
                .Split(separators.Select(s => s.StringValue[0]).ToArray(), splitOptions)
                .Select(s => new JSONValue(s))
                .ToList();
    }
}
