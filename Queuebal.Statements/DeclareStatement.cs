using Queuebal.Expressions;
using Queuebal.Json;

namespace Queuebal.Statements;

public class DeclareStatement : Statement
{
    /// <summary>
    /// The name of the variable to declare.
    /// </summary>
    public required string VariableName { get; set; }

    /// <summary>
    /// The initial value to assign to the variable.
    /// </summary>
    public IExpression InitialValue { get; set; } = new ValueExpression { Value = new JSONValue() };

    /// <summary>
    /// Executes the declare statement.
    /// </summary>
    /// <param name="context">The context in which the statement is executed.</param>
    protected override StatementBlockControl ExecuteStatement(StatementContext context)
    {
        var expressionContext = new ExpressionContext(context.DataProvider);
        if (context.DataProvider.GetValueInCurrentScope(VariableName) != null)
        {
            throw new InvalidOperationException($"Variable '{VariableName}' is already declared.");
        }

        var value = InitialValue.Evaluate(expressionContext, new());
        context.DataProvider.SetValue(VariableName, value);
        return StatementBlockControl.None;
    }
}
