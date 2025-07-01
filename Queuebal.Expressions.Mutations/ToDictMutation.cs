using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.Expressions.Mutations;

public class ToDictMutation : Mutation
{
    public static string MutationType => "ToDict";

    /// <summary>
    /// An expression used to select the key for each dictionary entry.
    /// This expression must evaluate to a string. If the KeySelector
    /// is not provided, the index of the current item in the list will
    /// be used as the key.
    /// </summary>
    public IExpression? KeySelector { get; set; }

    /// <summary>
    /// An expression used to select the value for each dictionary entry.
    /// If the ValueSelector is not provided, the item itself will be used as the value.
    /// </summary>
    public IExpression? ValueSelector { get; set; }

    /// <summary>
    /// A condition applied to each dictionary value to determine if it
    /// should be included in the dictionary.
    /// The input to the condition is a dictionary containing a single entry
    /// with the selected key, and the value.
    /// </summary>
    public ConditionExpression? Condition { get; set; }

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (inputValue.IsNull)
        {
            return new JSONValue(new Dictionary<string, JSONValue>());
        }

        if (!inputValue.IsList)
        {
            throw new InvalidOperationException("ToDictMutation can only be applied to a list input value.");
        }

        var output = new Dictionary<string, JSONValue>();

        int index = 0;
        foreach (var item in inputValue.ListValue)
        {
            string key = index.ToString();
            ++index;

            if (KeySelector != null)
            {
                // Evaluate the KeySelector expression to get the key for this item.
                var keyValue = KeySelector.Evaluate(context, item);
                if (!keyValue.IsString)
                {
                    throw new InvalidOperationException("KeySelector must evaluate to a string.");
                }
                key = keyValue.StringValue;
            }

            var value = item;
            if (ValueSelector != null)
            {
                // Evaluate the ValueSelector expression to get the value for this item.
                value = ValueSelector.Evaluate(context, item);
            }

            // If a condition is provided, evaluate it
            if (Condition != null)
            {
                var conditionInput = new JSONValue(new Dictionary<string, JSONValue>
                {
                    { "key", key },
                    { "value", value },
                });

                if (!Condition.Evaluate(context, conditionInput).BooleanValue)
                {
                    continue; // Skip this entry if the condition is not met
                }
            }

            output[key] = value;
        }

        return output;
    }
}