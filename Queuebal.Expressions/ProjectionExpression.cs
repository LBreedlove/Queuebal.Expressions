using Queuebal.Json;

namespace Queuebal.Expressions;


public class ProjectionExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "Projection";

    /// <summary>
    /// The expression used to get the items to be projected.
    /// </summary>
    public required IExpression Items { get; set; }

    /// <summary>
    /// A data map used as the source of values within the projection.
    /// </summary>
    public required Dictionary<string, IExpression> ContextDataMap { get; set; }

    /// <summary>
    /// An optional condition used to filter items before applying the projection.
    /// </summary>
    public ConditionExpression? ItemFilter { get; set; }

    /// <summary>
    /// The projection is used to transform each item in a collection by applying
    /// a set of field expressions, producing a new collection of objects with only
    /// the specified fields and computed values. It allows you to reshape or map input
    /// data into a new structure, similar to a "select" or "project" operation in query
    /// languages. Each field in the projection can be a constant, a variable, or another
    /// expression. The projection is applied to each item in the Items collection. The 
    /// item data is available in the context of the projection under the key "item".
    /// Additional context data can be provided through the ContextDataMap, which allows
    /// you to reference other values or expressions within the projection.
    /// </summary>
    public required Dictionary<string, IExpression> Projection { get; set; }

    /// <summary>
    /// Executes the projection expression on each item in the input list.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var items = Items.Evaluate(context, inputValue);
        if (items.IsNull)
        {
            return items; // If items is null, return null.
        }

        if (!items.IsList)
        {
            // If items didn't evaluate to a list, make it a list of 1 item.
            items = new JSONValue(new List<JSONValue> { items });
        }

        var expandedDataMap = new Dictionary<string, JSONValue>();
        foreach (var keyValuePair in ContextDataMap)
        {
            // Evaluate each key and value in the data map.
            var newValue = keyValuePair.Value.Evaluate(context, inputValue);

            // Add the evaluated key-value pair to the expanded data map.
            expandedDataMap[keyValuePair.Key] = newValue;
        }

        var results = new List<JSONValue>();
        foreach (var item in items.ListValue)
        {
            if (ItemFilter != null && !ItemFilter.Evaluate(context, item).BooleanValue)
            {
                // If the item does not pass the filter condition, skip it.
                continue;
            }

            var projectedItem = new Dictionary<string, JSONValue>();
            expandedDataMap["item"] = item;
            foreach (var keyValuePair in Projection)
            {
                var evaluatedItem = keyValuePair.Value.Evaluate(context, expandedDataMap);
                projectedItem[keyValuePair.Key] = evaluatedItem;
            }

            results.Add(projectedItem);
        }

        return results;
    }
}