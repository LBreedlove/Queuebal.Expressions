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
    /// The start of the range.
    /// </summary>
    public required int Start { get; set; }

    /// <summary>
    /// The number of elements in the range.
    /// </summary>
    public required int Count { get; set; }

    /// <summary>
    /// Indicates how the value should be incremented in the range.
    /// Defaults to 1.
    /// </summary>
    public int Step { get; set; } = 1;

    /// <summary>
    /// Evaluates the range expression and returns a list of integers from Start to End.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        if (Start < 0 || Count < 0)
        {
            throw new InvalidOperationException("Start and Count must be non-negative integers.");
        }

        var range = new List<JSONValue>(capacity: Count);

        int value = Start;
        for (int index = 0; index < Count; ++index)
        {
            range.Add(value);
            value += Step;
        }

        return new JSONValue(range);
    }
}