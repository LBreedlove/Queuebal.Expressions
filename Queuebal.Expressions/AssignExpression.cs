using Queuebal.Json;

namespace Queuebal.Expressions;


/// <summary>
/// Assigns a value to an existing variable in the VariableProvider.
/// </summary>
public class AssignExpression : Expression
{
    public static string ExpressionType { get; } = "Assign";

    /// <summary>
    /// The name of the variable to assign a value to.
    /// </summary>
    public required string VariableName { get; set; }

    /// <summary>
    /// The expression that evaluates to the value to assign to the variable.
    /// </summary>
    public required IExpression Value { get; set; }

    /// <summary>
    /// Executes the assignment statement.
    /// </summary>
    /// <param name="context">The context in which the statement is executed.</param>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var value = Value.Evaluate(context, inputValue);
        try
        {
            return context.VariableProvider.SetValue(VariableName, value);
        }
        catch (KeyNotFoundException)
        {
            throw new InvalidOperationException($"Variable '{VariableName}' is not declared.");
        }
    }
}
