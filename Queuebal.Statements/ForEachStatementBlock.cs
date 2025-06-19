using Queuebal.Expressions;
using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.Statements;


public class ForEachStatementBlock : Statement
{
    /// <summary>
    /// The expression used to get the values to iterate over.
    /// </summary>
    public required IExpression Values { get; set; }

    /// <summary>
    /// The name of the variable that will hold the current item in the iteration.
    /// </summary>
    public required string VariableName { get; set; }

    /// <summary>
    /// The statement block to execute for each item in the list.
    /// </summary>
    public required StatementBlock Statements { get; set; }

    /// <summary>
    /// Executes the statements for each item in the list.
    /// </summary>
    /// <param name="context">The context the statements are executed in.</param>
    protected override StatementBlockControl ExecuteStatement(StatementContext context)
    {
        var expressionContext = new ExpressionContext(context.DataProvider);
        var values = Values.Evaluate(expressionContext, new());
        if (!values.IsList)
        {
            throw new InvalidOperationException("ForEach statement block can only iterate over a list of values.");
        }

        var scopeValues = new Dictionary<string, JSONValue>
        {
            { VariableName, new() }
        };

        using (var scope = expressionContext.DataProvider.WithScope("foreach", scopeValues))
        {
            foreach (var item in values.ListValue)
            {
                // Set the current item in the context
                expressionContext.DataProvider.SetValue(VariableName, item);

                // Execute the statements for the current item
                var control = Statements.ExecuteBlock(context);

                // Check if we need to break or continue
                if (control == StatementBlockControl.Break)
                {
                    break;
                }

                if (control == StatementBlockControl.Return)
                {
                    return StatementBlockControl.Return;
                }
            }
        }

        return StatementBlockControl.None;
    }
}
