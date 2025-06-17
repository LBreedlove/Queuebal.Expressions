using Queuebal.Json;

namespace Queuebal.Expressions;


/// <summary>
/// An expression that applies a mapping expression to each item in a list.
/// </summary>
public class MapExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "Map";

    /// <summary>
    /// The expression to apply to each value in the source list.
    /// </summary>
    public required Expression Map { get; set; }

    /// <summary>
    /// Executes the map expression on each item in the input list.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsList)
        {
            throw new InvalidOperationException("Map expression can only be applied to a list input value.");
        }

        var results = new List<JSONValue>();
        foreach (var item in inputValue.ListValue)
        {
            // Evaluate the map expression for each item in the list.
            var evaluatedItem = Map.Evaluate(context, item);

            // Add the evaluated item to the result list.
            results.Add(evaluatedItem);
        }
        return results;
    }
}
