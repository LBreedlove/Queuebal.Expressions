using Queuebal.Json;

namespace Queuebal.Expressions;

/// <summary>
/// Represents an expression that returns a value.
/// </summary>
/// <remarks>
/// This expression does not use the inputValue in its evaluation.
/// </remarks>
public class ValueExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "Value";

    /// <summary>
    /// The value to be stored in and returned by this expression.
    /// </summary>
    public required JSONValue Value { get; set; }

    /// <summary>
    /// Returns the value stored in this expression.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        return Value;
    }
}
