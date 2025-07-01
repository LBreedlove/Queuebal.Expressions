using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class AddYearsMutation : Mutation
{
    public static string MutationType => "AddYears";

    /// <summary>
    /// An expression that evaluates to the number of years to add to the input date.
    /// This expression must evaluate to a number. If the value is a float, it will
    /// be cast to an int before being added to the date.
    /// </summary>
    public required IExpression Years { get; set; }

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsDateTime)
        {
            throw new InvalidOperationException("AddYearsMutation can only be applied to a DateTime input value.");
        }

        var years = Years.Evaluate(context, inputValue);
        if (!years.IsNumber)
        {
            throw new InvalidOperationException("The Years expression must evaluate to a number value.");
        }

        return inputValue.DateTimeValue.AddYears((int)years.IntValue);
    }
}
