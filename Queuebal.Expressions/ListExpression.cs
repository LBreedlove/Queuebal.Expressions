using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.Expressions;


/// <summary>
/// An expression that evaluates to a list of values.
/// Each entry in the list is evaluated independently, allowing for dynamic list creation based on expressions.
/// </summary>
public class ListExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "List";

    /// <summary>
    /// The list of entries evaluated to create the list.
    /// </summary>
    public required List<IExpression> Value { get; set; }

    /// <summary>
    /// Returns the list stored in this expression.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var output = new List<JSONValue>();
        foreach (var entry in Value)
        {
            var value = entry.Evaluate(context, inputValue);
            output.Add(value);
        }

        return output;
    }
}
