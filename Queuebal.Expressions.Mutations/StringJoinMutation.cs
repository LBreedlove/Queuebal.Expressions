using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class StringJoinMutation : Mutation
{
    public static string MutationType => "StringJoin";

    /// <summary>
    /// The separator used to join the strings in the inputValue.
    /// </summary>
    public required string Separator { get; set; }

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsList)
        {
            throw new InvalidOperationException("StringJoinMutation can only be applied to a list input value.");
        }

        if (inputValue.ListValue.Select(v => v.IsString).Any(v => !v))
        {
            throw new InvalidOperationException("StringJoinMutation can only be applied to a list of strings.");
        }

        return string.Join(Separator, inputValue.ListValue.Select(v => v.StringValue));
    }
}
