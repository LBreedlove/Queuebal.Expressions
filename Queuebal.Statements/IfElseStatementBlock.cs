using Queuebal.Expressions;

namespace Queuebal.Statements;


public class ConditionalStatementBlock
{
    /// <summary>
    /// The condition expression that determines whether the statement should be executed.
    /// This expression is evaluated to a boolean value.
    /// </summary>
    public required ConditionExpression Condition { get; set; }

    /// <summary>
    /// The value to evaluate the condition against.
    /// </summary>
    public required IExpression ConditionValue { get; set; }

    /// <summary>
    /// The statement block to execute if the condition is true.
    /// </summary>
    public required StatementBlock Statements { get; set; }

    /// <summary>
    /// Executes the statements if the condition evaluates to true.
    /// </summary>
    /// <param name="context">The context the statements are executed in.</param>
    /// <returns>true if the condition evaluated to true and the statements
    /// were executed, otherwise false.</returns>
    public Tuple<bool, StatementBlockControl> MaybeExecute(StatementContext context)
    {
        var expressionContext = new ExpressionContext(context.DataProvider);

        // Evaluate the condition
        var conditionResult = Condition.Evaluate
        (
            expressionContext,
            // there's no input value for the expression, so
            // it can only use variables from the context
            ConditionValue.Evaluate(expressionContext, new())
        ).BooleanValue;

        // If the condition is true, execute the statements
        if (conditionResult)
        {
            var control = Statements.ExecuteBlock(context);
            return new(true, control);
        }

        return new(false, StatementBlockControl.None);
    }
}

/// <summary>
/// Represents a block of statements that can be executed based on conditional logic.
/// This block contains multiple conditional statements (IfBlocks) that are evaluated in order.
/// If any of the conditions in the IfBlocks evaluate to true, the corresponding statement block is executed.
/// If none of the conditions are true, an optional ElseBlock can be executed.
/// </summary>
public class IfElseStatementBlock : Statement
{
    /// <summary>
    /// Gets the name of the statement type.
    /// </summary>
    public static string StatementType => "IfElse";

    /// <summary>
    /// The if statements to evaluate and possibly execute.
    /// </summary>
    public required List<ConditionalStatementBlock> IfBlocks { get; set; }

    /// <summary>
    /// Gets or sets the statement block to execute if all the conditions are false.
    /// </summary>
    public StatementBlock? ElseBlock { get; set; }

    /// <summary>
    /// Executes the conditional statements in the given context.
    /// It evaluates each condition in the IfBlocks and executes the corresponding
    /// statement block if the condition is true. If no conditions are true and an
    /// ElseBlock is defined, it executes the ElseBlock.
    /// If a condition is met, it returns the control from that block.
    /// If no conditions are met and no ElseBlock is defined, it returns StatementBlockControl.None,
    /// indicating that no further action is needed.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override StatementBlockControl ExecuteStatement(StatementContext context)
    {
        // Evaluate the conditions and execute the first block with a true condition.
        foreach (var block in IfBlocks)
        {
            var (conditionMet, control) = block.MaybeExecute(context);
            if (conditionMet)
            {
                return control; // Return the control from the executed block if the condition was met.
            }
        }

        // If no conditions were met and an ElseBlock is defined, execute it.
        if (ElseBlock != null)
        {
            return ElseBlock.ExecuteBlock(context);
        }

        return StatementBlockControl.None;
    }
}