using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class StringJoinMutation : Mutation
{
    public static string MutationType => "StringJoin";

    /// <summary>
    /// The separator used to join the strings in the inputValue.
    /// </summary>
    public required IExpression Separator { get; set; }

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue originalValue, JSONValue sourceData)
    {
        if (!originalValue.IsList)
        {
            throw new InvalidOperationException("StringJoinMutation can only be applied to a list input value.");
        }

        if (originalValue.ListValue.Select(v => v.IsString).Any(v => !v))
        {
            throw new InvalidOperationException("StringJoinMutation can only be applied to a list of strings.");
        }

        var separatorValue = Separator.Evaluate(context, sourceData);
        if (!separatorValue.IsString)
        {
            throw new InvalidOperationException("The Separator expression must evaluate to a string value.");
        }

        return string.Join(separatorValue.StringValue, originalValue.ListValue.Select(v => v.StringValue));
    }
}
