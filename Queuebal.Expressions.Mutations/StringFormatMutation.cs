using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.Expressions.Mutations;


public class StringFormatMutation : Mutation
{
    public static string MutationType => "StringFormat";

    /// <summary>
    /// The values to use to replace the format variables in the string.
    /// </summary>
    public required Dictionary<string, IExpression> Values { get; set; }

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue originalValue, JSONValue sourceData)
    {
        if (!originalValue.IsString)
        {
            throw new InvalidOperationException("StringFormatMutation can only be applied to a string input value.");
        }

        var formatValues = new Dictionary<string, JSONValue>();
        foreach (var kvp in Values)
        {
            var evaluatedValue = kvp.Value.Evaluate(context, sourceData);
            formatValues[kvp.Key] = evaluatedValue.ToString();
        }

        using (var scope = context.VariableProvider.WithScope("StringFormat", formatValues))
        {
            return VariableReplacement.Evaluate(originalValue.StringValue, context.VariableProvider);
        }
    }
}