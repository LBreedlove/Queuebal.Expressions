using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class AddSecondsMutation : Mutation
{
    public static string MutationType => "AddSeconds";

    /// <summary>
    /// An expression that evaluates to the number of seconds to add to the input date.
    /// This expression must evaluate to a number.
    /// </summary>
    public required IExpression Seconds { get; set; }

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsDateTime)
        {
            throw new InvalidOperationException("AddSecondsMutation can only be applied to a DateTime input value.");
        }

        var seconds = Seconds.Evaluate(context, inputValue);
        if (!seconds.IsNumber)
        {
            throw new InvalidOperationException("The Seconds expression must evaluate to a number value.");
        }

        return inputValue.DateTimeValue.AddSeconds(seconds.FloatValue);
    }
}
