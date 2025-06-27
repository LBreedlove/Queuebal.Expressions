using Queuebal.Json;

namespace Queuebal.Expressions;


/// <summary>
/// An expression that filters a list based on a specified condition.
/// If the result of the condition is true for an item, that item is included in the output list.
/// </summary>
public class FilterExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "Filter";

    /// <summary>
    /// The condition expression used to determine which items to include in the output list.
    /// </summary>
    public required ConditionExpression Condition { get; set; }

    /// <summary>
    /// Executes the map expression on each item in the input list.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsList && !inputValue.IsDict)
        {
            throw new InvalidOperationException("Filter expression can only be applied to a list or dict input value.");
        }

        if (inputValue.IsDict)
        {
            return FilterDict(context, inputValue);
        }

        var results = new List<JSONValue>();
        foreach (var item in inputValue.ListValue)
        {
            // ConditionExpression is guaranteed to evaluate to a boolean value.
            if (Condition.Evaluate(context, item).BooleanValue)
            {
                // If the condition evaluates to true, include the item in the results.
                results.Add(item);
            }
        }

        return results;
    }

    private JSONValue FilterDict(ExpressionContext context, JSONValue inputValue)
    {
        var output = new Dictionary<string, JSONValue>();
        foreach (var kvp in inputValue.DictValue)
        {
            var conditionValue = new Dictionary<string, JSONValue>
            {
                { "key", kvp.Key },
                { "value", kvp.Value },
            };

            if (Condition.Evaluate(context, conditionValue).BooleanValue)
            {
                output.Add(kvp.Key, kvp.Value);
            }
        }

        return output;
    }
}
