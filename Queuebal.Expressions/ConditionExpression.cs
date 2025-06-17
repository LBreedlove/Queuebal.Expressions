using Queuebal.Json;

namespace Queuebal.Expressions;

public class ConditionExpression : Expression
{
    /// <summary>
    ///  Represents the type of expression this class implements.
    /// </summary>
    public static string ExpressionType { get; } = "Condition";

    /// <summary>
    /// The condition set that this expression evaluates.
    /// </summary>
    public required ConditionSet ConditionSet { get; set; }

    /// <summary>
    /// Evaluates the condition expression and returns a boolean JSONValue indicating whether the condition is met.
    /// </summary>
    /// <param name="context">The context the expression is running in.</param>
    /// <param name="inputValue">The value to check the condition against.</param>
    /// <returns>true if the conditiono is met, otherwise false.</returns>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        return ConditionSet.Evaluate(context, inputValue);
    }
}
