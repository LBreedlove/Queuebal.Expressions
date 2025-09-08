using Queuebal.Json;

namespace Queuebal.Expressions;


/// <summary>
/// Declares a variable in the current VariableProviderScope and assigns it a value.
/// This expression is used to create a new variable and initialize it with a value.
/// If the variable already exists, an exception is thrown.
/// </summary>
public class DeclareAndAssignExpression : Expression
{
    public static string ExpressionType { get; } = "DeclareAndAssign";

    /// <summary>
    /// The name of the variable to declare and assign a value to.
    /// </summary>
    public required string VariableName { get; set; }

    /// <summary>
    /// The expression that evaluates to the value to assign to the variable.
    /// </summary>
    public required IExpression Value { get; set; }

    /// <summary>
    /// Executes the declaration and assignment statement.
    /// </summary>
    /// <param name="context">The context in which the statement is executed.</param>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        if (context.VariableProvider.GetValueInCurrentScope(VariableName) != null)
        {
            throw new InvalidOperationException($"Variable '{VariableName}' is already declared.");
        }

        var newValue = Value.Evaluate(context, inputValue);
        context.VariableProvider.AddValue(VariableName, newValue);
        return newValue;
    }
}