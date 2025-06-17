using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class AddHoursMutation : Mutation
{
    public static string MutationType => "AddHours";

    /// <summary>
    /// An expression that evaluates to the number of hours to add to the input date.
    /// This expression must evaluate to an integer value.
    /// </summary>
    public required IExpression Hours { get; set; }

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsDateTime)
        {
            throw new InvalidOperationException("AddHoursMutation can only be applied to a DateTime input value.");
        }

        var hours = Hours.Evaluate(context, inputValue);
        if (!hours.IsNumber)
        {
            throw new InvalidOperationException("The Hours expression must evaluate to a number value.");
        }

        return inputValue.DateTimeValue.AddHours(hours.FloatValue);
    }
}
