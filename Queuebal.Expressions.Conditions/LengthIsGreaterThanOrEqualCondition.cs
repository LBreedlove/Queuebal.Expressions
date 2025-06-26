using Queuebal.Expressions;
using Queuebal.Json;

namespace Queuebal.Expressions.Conditions;

/// <summary>
/// A condition that checks if the input value's length is greater than or equal to the comparer value,
/// or the length of the comparer value.
/// </summary>
public class LengthIsGreaterThanOrEqualCondition : BinaryCondition
{
    private static HashSet<JSONFieldType> _validInputTypes => new()
    {
        JSONFieldType.String,
        JSONFieldType.List,
        JSONFieldType.Dictionary,
    };

    /// <summary>
    /// Gets the name of the condition type.
    /// </summary>
    public static string ConditionType { get; } = "LengthIsGreaterThanOrEqual";

    /// <summary>
    /// Evaluates the condition by checking if the input value's length is greater than or equal the specified value.
    /// </summary>
    /// <param name="context">The context the condition is running in.</param>
    /// <param name="inputValue">The value to compare against.</param>
    /// <returns>true if the input value's length is greater than or equal the specified value, otherwise false.</returns>
    protected override bool EvaluateCondition(ExpressionContext context, JSONValue inputValue, JSONValue comparerValue)
    {
        if (inputValue.IsNull)
        {
            return false;
        }

        if (!_validInputTypes.Contains(inputValue.FieldType))
        {
            throw new InvalidOperationException("IsLengthGreaterThan inputValue must be a string, list, or dictionary");
        }

        int length = GetLength(inputValue);
        if (comparerValue.IsInteger)
        {
            return length >= comparerValue.IntValue;
        }

        return length >= GetLength(comparerValue);
    }

    /// <summary>
    /// Gets the length of the given JSONValue, if the value is a string, list, or dictionary.
    /// </summary>
    /// <param name="value">The value to get the length of.</param>
    /// <returns>The length of the value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the value is not a string, list, or dictionary.</exception>
    private static int GetLength(JSONValue value) => value.FieldType switch
    {
        JSONFieldType.String => value.StringValue.Length,
        JSONFieldType.List => value.ListValue.Count,
        JSONFieldType.Dictionary => value.DictValue.Count,
        _ => throw new InvalidOperationException("IsLengthGreater than can only accept a string, list, or dictionary")
    };
}
