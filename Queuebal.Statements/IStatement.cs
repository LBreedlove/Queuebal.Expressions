namespace Queuebal.Statements;


/// <summary>
/// Indicates how a statement should impact the execution of a statement block.
/// </summary>
public enum StatementBlockControl
{
    None,
    Break,
    Continue,
    Return,
}


/// <summary>
/// Interface for a statement that can be executed within a statement block.
/// </summary>
public interface IStatement
{
    /// <summary>
    /// Gets the name of the statement type.
    /// </summary>
    public static string StatementType => throw new NotImplementedException("StatementType must be implemented in derived classes.");

    /// <summary>
    /// Evaluates the statement and returns a value indicating how
    /// the statement block's execution should be continue.
    /// </summary>
    StatementBlockControl Execute(StatementContext context);
}

/// <summary>
/// Represents a statement that can be executed within a statement block.
/// This class implements the IStatement interface and provides a base for all statements.
/// It includes a method to execute the statement and a property to indicate if the statement
/// can break out of the parent statement block's execution.
/// Derived classes should implement the ExecuteStatement method to define the specific behavior
/// of the statement.
/// The CanBreak property can be overridden to specify if the statement can cause an exit
/// from the block's execution.
/// </summary>
public abstract class Statement : IStatement
{
    /// <summary>
    /// Executes the statement in the given context.
    /// </summary>
    /// <param name="context">The context the statement is executing in.</param>
    /// <returns>
    /// True if the statement should trigger an exit from the parent statement block's execution,
    /// otherwise false.
    /// </returns>
    public StatementBlockControl Execute(StatementContext context)
    {
        // Execute the statement logic and return true if it should break out of the block.
        return ExecuteStatement(context);
    }

    /// <summary>
    /// Method to be implemented by derived classes to execute the statement.
    /// </summary>
    /// <param name="context">The context the statement is running in.</param>
    /// <returns>
    /// True if the statement should trigger an exit from the parent statement block's execution,
    /// otherwise false.
    /// </returns>
    protected abstract StatementBlockControl ExecuteStatement(StatementContext context);
}

/// <summary>
/// A block of statements that can be executed sequentially within a loop.
/// </summary>
public class StatementBlock
{
    /// <summary>
    /// Gets or sets the list of statements in the block.
    /// </summary>
    public required List<IStatement> Statements { get; set; }

    /// <summary>
    /// Executes the statements in the block and returns a control value indicating how
    /// the execution of the block should continue.
    /// </summary>
    public StatementBlockControl ExecuteBlock(StatementContext context)
    {
        // Execute all statements in the block.
        foreach (var statement in Statements)
        {
            var result = statement.Execute(context);
            if (result != StatementBlockControl.None)
            {
                return result; // Return control to the caller based on the statement's execution result.
            }
        }
        return StatementBlockControl.None; // Continue execution if no statements triggered a control change.
    }
}
