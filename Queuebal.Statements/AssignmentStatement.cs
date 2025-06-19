using Queuebal.Expressions;

namespace Queuebal.Statements;


public class AssignmentStatement : Statement
{
    /// <summary>
    /// The name of the variable to assign a value to.
    /// </summary>
    public required string VariableName { get; set; }

    /// <summary>
    /// The expression that evaluates to the value to assign to the variable.
    /// </summary>
    public required IExpression ValueExpression { get; set; }

    /// <summary>
    /// Executes the assignment statement.
    /// </summary>
    /// <param name="context">The context in which the statement is executed.</param>
    protected override StatementBlockControl ExecuteStatement(StatementContext context)
    {
        var expressionContext = new ExpressionContext(context.DataProvider);
        var newValue = ValueExpression.Evaluate(expressionContext, new());
        context.DataProvider.SetValue(VariableName, newValue);
        return StatementBlockControl.None;
    }
}
