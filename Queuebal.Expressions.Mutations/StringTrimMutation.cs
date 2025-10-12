using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class StringTrimMutation : Mutation
{
    public static string MutationType => "StringTrim";

    /// <summary>
    /// Indicates if whitespace from the beginning of the string should
    /// be removed. Default true.
    /// </summary>
    public bool TrimStart { get; set; } = true;

    /// <summary>
    /// Indicates if whitespace from the end of the string should
    /// be removed. Default true.
    /// </summary>
    public bool TrimEnd { get; set; } = true;

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue originalValue, JSONValue sourceData)
    {
        if (!originalValue.IsString)
        {
            throw new InvalidOperationException("StringTrimMutation can only be applied to a string input value.");
        }

        if (TrimStart && TrimEnd)
        {
            return originalValue.StringValue.Trim();
        }

        if (TrimStart)
        {
            return originalValue.StringValue.TrimStart();
        }

        if (TrimEnd)
        {
            return originalValue.StringValue.TrimEnd();
        }

        return originalValue.StringValue;
    }
}
