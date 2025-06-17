using Queuebal.Expressions;
using Queuebal.Json;

namespace Queuebal.Expressions.Conditions;

/// <summary>
/// A condition that checks if the input value equals the comparer value.
/// </summary>
public class EqualsCondition : BinaryCondition
{
    /// <summary>
    /// Gets the name of the condition type.
    /// </summary>
    public static string ConditionType { get; } = "Equals";

    /// <summary>
    /// Evaluates the condition by checking if the input value equals the specified value.
    /// </summary>
    /// <param name="context">The context the condition is running in.</param>
    /// <param name="inputValue">The value to compare against.</param>
    /// <returns>true if the input value equals the specified value, otherwise false.</returns>
    protected override bool EvaluateCondition(ExpressionContext context, JSONValue inputValue, JSONValue comparerValue) =>
        inputValue.Equals(comparerValue);
}
