using Queuebal.Json;

namespace Queuebal.Expressions;

public class MutationExpression : Expression
{
    /// <summary>
    ///  Represents the type of expression this class implements.
    /// </summary>
    public static string ExpressionType { get; } = "Mutation";

    /// <summary>
    /// The mutation this expression evaluates.
    /// </summary>
    public required IMutation Mutation { get; set; }

    /// <summary>
    /// Evaluates the mutation expression and returns the JSONValue result.
    /// </summary>
    /// <param name="context">The context the expression is running in.</param>
    /// <param name="inputValue">The value to mutate.</param>
    /// <returns>The mutated value.</returns>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        return Mutation.Evaluate(context, inputValue);
    }
}
