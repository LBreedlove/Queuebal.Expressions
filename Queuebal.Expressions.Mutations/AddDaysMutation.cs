using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class AddDaysMutation : Mutation
{
    public static string MutationType => "AddDays";

    /// <summary>
    /// An expression that evaluates to the number of days to add to the input date.
    /// This expression must evaluate to a number.
    /// </summary>
    public required IExpression Days { get; set; }

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsDateTime)
        {
            throw new InvalidOperationException("AddDaysMutation can only be applied to a DateTime input value.");
        }

        var days = Days.Evaluate(context, inputValue);
        if (!days.IsNumber)
        {
            throw new InvalidOperationException("The Days expression must evaluate to a number value.");
        }

        return inputValue.DateTimeValue.AddDays(days.FloatValue);
    }
}
