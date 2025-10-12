using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class ToUpperMutation : Mutation
{
    public static string MutationType => "ToUpper";

    /// <summary>
    /// Indicates if the string should be converted to uppercase
    /// using the casing rules of the invariant culture.
    /// </summary>
    public IExpression UseInvariantCulture { get; set; } = new ValueExpression { Value = new JSONValue(true) };

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue originalValue, JSONValue sourceData)
    {
        if (!originalValue.IsString)
        {
            throw new InvalidOperationException("ToUpperMutation can only be applied to a string input value.");
        }

        var useInvariantCulture = UseInvariantCulture.Evaluate(context, sourceData);
        if (!useInvariantCulture.IsBoolean)
        {
            throw new InvalidOperationException("The UseInvariantCulture expression must evaluate to a boolean value.");
        }

        if (useInvariantCulture.BooleanValue)
        {
            return originalValue.StringValue.ToUpperInvariant();
        }

        // If UseInvariantCulture is false, use the current culture
        return originalValue.StringValue.ToUpper();
    }
}
