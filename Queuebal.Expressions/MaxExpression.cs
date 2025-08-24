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
    /// The values to compare.
    /// </summary>
    public required IExpression Values { get; set; }

    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var values = Values.Evaluate(context, inputValue);
        if (!values.IsList || !values.ListValue.Any())
        {
            throw new InvalidOperationException("Max values must evaluate to a non-empty list.");
        }

        var maxValue = values.ListValue[0];
        if (!maxValue.IsNumber && !maxValue.IsString)
        {
            throw new InvalidOperationException("Max can only compare numerical and string values");
        }

        foreach (var value in values.ListValue.Skip(1))
        {
            if (value.FieldType != maxValue.FieldType)
            {
                throw new InvalidOperationException("Max can only compare values of the same type");
            }

            maxValue = MaxValue(maxValue, value);
        }

        return maxValue;
    }

    private JSONValue MaxValue(JSONValue lvalue, JSONValue rvalue)
    {
        if (lvalue.IsNumber)
        {
            return Math.Max(lvalue.FloatValue, rvalue.FloatValue);
        }

        return lvalue.StringValue.CompareTo(rvalue.StringValue) > 0
            ? lvalue.StringValue
            : rvalue.StringValue;
    }
}
