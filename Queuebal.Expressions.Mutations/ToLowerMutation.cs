using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class ToLowerMutation : Mutation
{
    public static string MutationType => "ToLower";

    /// <summary>
    /// Indicates if the string should be converted to lowercase
    /// using the casing rules of the invariant culture.
    /// </summary>
    public bool UseInvariantCulture { get; set; } = true;

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsString)
        {
            throw new InvalidOperationException("ToLowerMutation can only be applied to a string input value.");
        }

        if (UseInvariantCulture)
        {
            return inputValue.StringValue.ToLowerInvariant();
        }

        // If UseInvariantCulture is false, use the current culture
        return inputValue.StringValue.ToLower();
    }
}
