using Queuebal.Json;

namespace Queuebal.Expressions;

/// <summary>
/// An expression that generates a range of integers.
/// </summary>
public class RangeExpression : Expression
{
    /// <summary>
    /// The type of expression this class represents.
    /// </summary>
    public static string ExpressionType { get; } = "Range";

    /// <summary>
    /// The start of the range. This expression must evaluate to a number.
    /// </summary>
    public required IExpression Start { get; set; }

    /// <summary>
    /// The number of elements in the range. This expression must evaluate to a number.
    /// </summary>
    public required IExpression Count { get; set; }

    /// <summary>
    /// Indicates how the value should be incremented in the range. This expression must evaluate to a number.
    /// Defaults to 1.
    /// </summary>
    public IExpression Step { get; set; } = new ValueExpression { Value = 1};

    /// <summary>
    /// Evaluates the range expression and returns a list of integers from Start to End.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var start = Start.Evaluate(context, inputValue);
        if (!start.IsNumber)
        {
            throw new InvalidOperationException("Range Start must evaluate to a number");
        }

        var count = Count.Evaluate(context, inputValue);
        if (!count.IsNumber)
        {
            throw new InvalidOperationException("Range Count must evaluate to a number");
        }

        var step = Step.Evaluate(context, inputValue);
        if (!step.IsNumber)
        {
            throw new InvalidOperationException("Range Step must evaluate to a number");
        }

        if (start.IntValue < 0 || count.IntValue < 0)
        {
            throw new InvalidOperationException("Range Start and Count must be non-negative integers.");
        }

        var range = new List<JSONValue>(capacity: (int)count.IntValue);

        int value = (int)start.IntValue;
        for (int index = 0; index < (int)count.IntValue; ++index)
        {
            range.Add(value);
            value += (int)step.IntValue;
        }

        return new JSONValue(range);
    }
}