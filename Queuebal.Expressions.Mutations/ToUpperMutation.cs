using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class ToUpperMutation : Mutation
{
    public static string MutationType => "ToUpper";

    /// <summary>
    /// Indicates if the string should be converted to uppercase
    /// using the casing rules of the invariant culture.
    /// </summary>
    public bool UseInvariantCulture { get; set; } = true;

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsString)
        {
            throw new InvalidOperationException("ToUpperMutation can only be applied to a string input value.");
        }

        if (UseInvariantCulture)
        {
            return inputValue.StringValue.ToUpperInvariant();
        }

        // If UseInvariantCulture is false, use the current culture
        return inputValue.StringValue.ToUpper();
    }
}
