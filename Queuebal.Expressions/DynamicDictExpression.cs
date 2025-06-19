using Queuebal.Json;

namespace Queuebal.Expressions;


public class DynamicDictEntry
{
    /// <summary>
    /// The key for the dictionary entry.
    /// </summary>
    public required IExpression Key { get; set; }

    /// <summary>
    /// The value for the dictionary entry.
    /// </summary>
    public required IExpression Value { get; set; }

    /// <summary>
    /// An optional condition that indicates if the
    /// entry should be included in the dictionary.
    /// </summary>
    /// <remarks
    /// The input to the condition is the dictionary entry
    /// in the form of a dictionary with a single key-value pair:
    /// ```
    /// {
    ///   "<evaluated key>": "<evaluated value>"
    /// }
    /// ```
    /// </remarks>
    public ConditionExpression? Condition { get; set; }
}


public class DynamicDictExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "DynamicDict";

    /// <summary>
    /// The list of dictionary entries evaluated to create the dictionary.
    /// </summary>
    public required List<DynamicDictEntry> Entries { get; set; }

    /// <summary>
    /// Returns the dictionary stored in this expression.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var output = new Dictionary<string, JSONValue>();
        foreach (var entry in Entries)
        {
            // Evaluate the key and value expressions for each entry.
            var key = entry.Key.Evaluate(context, inputValue);
            if (!key.IsString)
            {
                throw new InvalidOperationException("DynamicDict keys must evaluate to a string.");
            }

            var value = entry.Value.Evaluate(context, inputValue);
            if (entry.Condition != null)
            {
                // Evaluate the condition for the entry.
                var conditionInput = new Dictionary<string, JSONValue>
                {
                    { key.StringValue, value }
                };

                // If the condition evaluates to false, skip this entry.
                if (!entry.Condition.Evaluate(context, new JSONValue(conditionInput)).BooleanValue)
                {
                    continue;
                }
            }

            // Add the evaluated key-value pair to the dictionary.
            output[key.StringValue] = value;
        }

        return output;
    }
}
