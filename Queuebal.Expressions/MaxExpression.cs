using Queuebal.Json;

namespace Queuebal.Expressions;


/// <summary>
/// Represents an expression that returns the minimum value from two values.
/// </summary>
public class MaxExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "Max";

    /// <summary>
    /// The first value to compare.
    /// </summary>
    public required IExpression LValue { get; set; }

    /// <summary>
    /// The second value to compare.
    /// </summary>
    public required IExpression RValue { get; set; }

    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var lvalue = LValue.Evaluate(context, inputValue);
        if (!lvalue.IsNumber && !lvalue.IsString)
        {
            throw new InvalidOperationException("Max can only compare numerical and string values");
        }

        var rvalue = RValue.Evaluate(context, inputValue);
        if (!rvalue.IsNumber && !rvalue.IsString)
        {
            throw new InvalidOperationException("Max can only compare numerical and string values");
        }

        if (lvalue.IsNumber && !rvalue.IsNumber)
        {
            throw new InvalidOperationException("Max can only compare values of the same type");
        }

        if (lvalue.IsString && !rvalue.IsString)
        {
            throw new InvalidOperationException("Max can only compare values of the same type");
        }

        if (lvalue.IsNumber)
        {
            return Math.Max(lvalue.FloatValue, rvalue.FloatValue);
        }

        return lvalue.StringValue.CompareTo(rvalue.StringValue) > 0
            ? lvalue.StringValue
            : rvalue.StringValue;
    }
}
