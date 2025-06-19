using Queuebal.Json;

namespace Queuebal.Expressions;


/// <summary>
/// The individual entries for an if-else expression.
/// </summary>
public class IfElseExpressionCondition
{
    public required ConditionExpression Condition { get; set; }
    public required IExpression IfTrue { get; set; }
}


/// <summary>
/// An expression that filters a list based on a specified condition.
/// If the result of the condition is true for an item, that item is included in the output list.
/// </summary>
public class IfElseExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "IfElse";

    /// <summary>
    /// The conditions expressions to evaluate, in order, to determine which branch's expression to return.
    /// </summary>
    public required List<IfElseExpressionCondition> Branches { get; set; }

    /// <summary>
    /// The value to return if no branches match.
    /// </summary>
    public IExpression? ElseValue { get; set; }

    /// <summary>
    /// Evaluates the if-else expressions based on their condition, returning the first
    /// expression with a condition that evaluates to true.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        foreach (var branch in Branches)
        {
            if (branch.Condition.Evaluate(context, inputValue).BooleanValue)
            {
                // If the condition evaluates to true, return the corresponding expression.
                return branch.IfTrue.Evaluate(context, inputValue);
            }
        }

        if (ElseValue != null)
        {
            // If no conditions matched and an ElseValue is provided, evaluate and return it.
            return ElseValue.Evaluate(context, inputValue);
        }

        return new JSONValue();
    }
}
