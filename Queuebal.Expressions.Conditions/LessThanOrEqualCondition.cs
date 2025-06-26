using Queuebal.Json;

namespace Queuebal.Expressions.Conditions;

/// <summary>
/// A condition that checks if the input value is less than or equal to the comparer value.
/// </summary>
public class LessThanOrEqualCondition : BinaryCondition
{
    /// <summary>
    /// Gets the name of the condition type.
    /// </summary>
    public static string ConditionType { get; } = "LessThanOrEqual";

    /// <summary>
    /// Evaluates the condition by checking if the input value is less than or equal to the specified value.
    /// </summary>
    /// <param name="context">The context the condition is running in.</param>
    /// <param name="inputValue">The value to compare against.</param>
    /// <returns>true if the input value is less than or equal to the specified value, otherwise false.</returns>
    protected override bool EvaluateCondition(ExpressionContext context, JSONValue inputValue, JSONValue comparerValue)
    {
        if (inputValue.IsNumber && !comparerValue.IsNumber)
        {
            throw new InvalidOperationException("LessThanOrEqual can only compare values of the same type");
        }

        if (inputValue.IsString && !comparerValue.IsString)
        {
            throw new InvalidOperationException("LessThanOrEqual can only compare values of the same type");
        }

        if (inputValue.IsNumber)
        {
            return inputValue.FloatValue <= comparerValue.FloatValue;
        }

        return inputValue.StringValue.CompareTo(comparerValue.StringValue) <= 0;
    }
}
