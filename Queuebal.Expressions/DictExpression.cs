using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.Expressions;


/// <summary>
/// An expression that creates a dictionary from a set of key-value pairs.
/// Each key is evaluated to a string, and each value is evaluated to a JSONValue.
/// The resulting dictionary is returned as a JSONValue.
/// </summary>
public class DictExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "Dict";

    /// <summary>
    /// The list of dictionary entries evaluated to create the dictionary.
    /// </summary>
    public required Dictionary<string, IExpression> Value { get; set; }

    /// <summary>
    /// Returns the dictionary stored in this expression.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var output = new Dictionary<string, JSONValue>();
        foreach (var kv in Value)
        {
            var key = Tokenizer.Evaluate(kv.Key, context.VariableProvider);
            if (!key.IsString)
            {
                throw new InvalidOperationException("Dict keys must evaluate to a string.");
            }

            var value = kv.Value.Evaluate(context, inputValue);
            output[key.StringValue] = value;
        }

        return output;
    }
}
