namespace Queuebal.Statements;

public class BreakStatement : Statement
{
    /// <summary>
    /// Executes the break statement.
    /// </summary>
    /// <param name="context">The context in which the statement is executed.</param>
    /// <returns>Returns StatementBlockControl.Break to indicate that the loop should be exited.</returns>
    protected override StatementBlockControl ExecuteStatement(StatementContext context)
    {
        // The break statement simply indicates that the current loop should be exited.
        return StatementBlockControl.Break;
    }
}

public class ContinueStatement : Statement
{
    /// <summary>
    /// Executes the continue statement.
    /// </summary>
    /// <param name="context">The context in which the statement is executed.</param>
    /// <returns>Returns StatementBlockControl.Continue to indicate that the loop should continue to the next iteration.</returns>
    protected override StatementBlockControl ExecuteStatement(StatementContext context)
    {
        // The continue statement indicates that the current iteration should be skipped and the next one started.
        return StatementBlockControl.Continue;
    }
}

public class ReturnStatement : Statement
{
    /// <summary>
    /// Executes the return statement.
    /// </summary>
    /// <param name="context">The context in which the statement is executed.</param>
    /// <returns>Returns StatementBlockControl.Return to indicate that the function or block should return.</returns>
    protected override StatementBlockControl ExecuteStatement(StatementContext context)
    {
        // The return statement indicates that the current function or block should return.
        return StatementBlockControl.Return;
    }
}