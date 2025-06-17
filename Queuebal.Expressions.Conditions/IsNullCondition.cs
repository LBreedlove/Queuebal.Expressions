using Queuebal.Expressions;
using Queuebal.Json;

namespace Queuebal.Expressions.Conditions;

/// <summary>
/// A condition that checks if the input value is null.
/// </summary>
public class IsNullCondition : UnaryCondition
{
    /// <summary>
    /// Gets the name of the condition type.
    /// </summary>
    public static string ConditionType { get; } = "IsNull";

    /// <summary>
    /// Determines if the input value is null.
    /// </summary>
    /// <param name="context">The context the condition is running in.</param>
    /// <param name="inputValue">The value to check for null.</param>
    /// <returns>true if the input value is null, otherwise false.</returns>
    protected override bool EvaluateCondition(ExpressionContext context, JSONValue inputValue) =>
        inputValue.IsNull;
}
