using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class AddMinutesMutation : Mutation
{
    public static string MutationType => "AddMinutes";

    /// <summary>
    /// An expression that evaluates to the number of minutes to add to the input date.
    /// This expression must evaluate to a number.
    /// </summary>
    public required IExpression Minutes { get; set; }

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsDateTime)
        {
            throw new InvalidOperationException("AddMinutesMutation can only be applied to a DateTime input value.");
        }

        var minutes = Minutes.Evaluate(context, inputValue);
        if (!minutes.IsNumber)
        {
            throw new InvalidOperationException("The Minutes expression must evaluate to a number value.");
        }

        return inputValue.DateTimeValue.AddMinutes(minutes.FloatValue);
    }
}
