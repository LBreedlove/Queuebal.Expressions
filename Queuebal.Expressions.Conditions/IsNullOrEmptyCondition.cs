using Queuebal.Json;

namespace Queuebal.Expressions.Conditions;

/// <summary>
/// A condition that checks if the input value is null.
/// </summary>
public class IsNullOrEmptyCondition : UnaryCondition
{
    /// <summary>
    /// Gets the name of the condition type.
    /// </summary>
    public static string ConditionType { get; } = "IsNullOrEmpty";

    /// <summary>
    /// Determines if the input value is null or an empty container.
    /// </summary>
    /// <param name="context">The context the condition is running in.</param>
    /// <param name="inputValue">The value to check for null or empty.</param>
    /// <returns>true if the input value is null or empty, otherwise false.</returns>
    protected override bool EvaluateCondition(ExpressionContext context, JSONValue inputValue)
    {
        if (inputValue.IsNull)
        {
            return true;
        }

        return inputValue.FieldType switch
        {
            JSONFieldType.String => string.IsNullOrEmpty(inputValue.StringValue),
            JSONFieldType.List => inputValue.ListValue.Count == 0,
            JSONFieldType.Dictionary => inputValue.DictValue.Count == 0,
            _ => false,
        };
    }
}
