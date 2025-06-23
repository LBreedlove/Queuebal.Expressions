using Queuebal.Json;

namespace Queuebal.Expressions;


/// <summary>
/// Represents an expression that returns the first non-null item in a list of expressions.
/// </summary>
public class CoalesceExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "Coalesce";

    /// <summary>
    /// The list of expressions to return the first non-null value from.
    /// </summary>
    public required List<IExpression> Values { get; set; }

    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        foreach (var expression in Values)
        {
            var value = expression.Evaluate(context, inputValue);
            if (!value.IsNull)
            {
                return value;
            }
        }

        return new();
    }
}
