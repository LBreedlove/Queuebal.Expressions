using Queuebal.Json;

namespace Queuebal.Expressions;


/// <summary>
/// Represents an expression that counts the items in a string, list, or dictionary value.
/// </summary>
public class CountExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "Count";

    /// <summary>
    /// Indicates whether or not 'falsey' elements should be excluded from the count.
    /// Falsey elements:
    ///   * A boolean field with a value of false.
    ///   * An empty list
    ///   * An empty string
    ///   * An empty dictionary
    ///   * A number value of 0
    /// </summary>
    public bool ExcludeFalsey { get; set; } = false;

    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        return ExcludeFalsey
            ? CountNonFalseyValues(inputValue)
            : CountValues(inputValue);
    }

    /// <summary>
    /// Counts the number of non-Falsey values in the Container type.
    /// </summary>
    /// <param name="inputValue">The value to count the items in.</param>
    /// <returns>The number of non-falsey items in the value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a non-string/list/dict value is provided.</exception>
    private int CountNonFalseyValues(JSONValue inputValue) => inputValue.FieldType switch
    {
        JSONFieldType.String => inputValue.StringValue.Length,
        JSONFieldType.List => inputValue.ListValue.Where(i => !IsFalsey(i)).Count(),
        JSONFieldType.Dictionary => inputValue.DictValue.Where(kv => !IsFalsey(kv.Value)).Count(),
        _ => throw new InvalidOperationException("CountExpression cannot be executed on a non-container type")
    };

    /// <summary>
    /// Counts the number of values in the Container type.
    /// </summary>
    /// <param name="inputValue">The value to count the items in.</param>
    /// <returns>The number of items in the value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a non-string/list/dict value is provided.</exception>
    private int CountValues(JSONValue inputValue) => inputValue.FieldType switch
    {
        JSONFieldType.String => inputValue.StringValue.Length,
        JSONFieldType.List => inputValue.ListValue.Count,
        JSONFieldType.Dictionary => inputValue.DictValue.Count,
        _ => throw new InvalidOperationException("CountExpression cannot be executed on a non-container type")
    };

    /// <summary>
    /// Indicates if the given JSONValue is 'falsey'.
    /// </summary>
    /// <param name="value">The value to check for a 'falsey' value.</param>
    /// <returns>true if the value is 'falsey', otherwise false.</returns>
    private bool IsFalsey(JSONValue value) => value.FieldType switch
    {
        JSONFieldType.Null => true,
        JSONFieldType.String => string.IsNullOrEmpty(value.StringValue),
        JSONFieldType.Boolean => !value.BooleanValue,
        JSONFieldType.Integer => value.IntValue == 0,
        JSONFieldType.Float => value.FloatValue == 0.0f,
        JSONFieldType.List => value.ListValue.Count == 0,
        JSONFieldType.Dictionary => value.DictValue.Count == 0,
        _ => false,
    };
}
