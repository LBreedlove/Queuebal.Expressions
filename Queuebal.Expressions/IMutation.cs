using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.Expressions;


/// <summary>
/// The base interface all mutations must implement.
/// </summary>
public interface IMutation
{
    /// <summary>
    /// Gets the name of the mutation type.
    /// </summary>
    public static string MutationType => throw new NotImplementedException("MutationType must be implemented in derived classes.");

    /// <summary>
    /// The value to mutate.
    /// </summary>
    IExpression? OriginalValue { get; set; }

    /// <summary>
    /// Evaluates the mutation and returns the JSONValue result.
    /// </summary>
    JSONValue Evaluate(ExpressionContext context, JSONValue inputValue);
}

/// <summary>
/// The base class for mutations.
/// </summary>
public abstract class Mutation : IMutation
{
    /// <summary>
    /// An input expression that can be used to mutate the input value,
    /// before the mutation is evaluated. This is useful for chaining mutations.
    /// </summary>
    public IExpression? OriginalValue { get; set; }

    /// <summary>
    /// Evaluates the mutation and returns the JSONValue result.
    /// </summary>
    /// <param name="context">The context the mutation is running in.</param>
    /// <param name="inputValue">The value to be mutated is evaluated against.</param>
    /// <returns></returns>
    public JSONValue Evaluate(ExpressionContext context, JSONValue inputValue)
    {
        var originalValue = inputValue;
        if (OriginalValue != null)
        {
            // If an OriginalValue is provided, evaluate it first
            originalValue = OriginalValue.Evaluate(context, inputValue);
        }

        // Evaluate the mutation using the provided context and input value
        var result = EvaluateMutation(context, originalValue, inputValue);
        if (result.IsString)
        {
            // If the result is a string, evaluate it as a token
            result = VariableReplacement.Evaluate(result.StringValue, context.VariableProvider);
        }
        return result;
    }

    /// <summary>
    /// Method to be implemented by derived classes to evaluate the mutation.
    /// </summary>
    /// <param name="context">The context the mutation is running in.</param>
    /// <param name="inputValue">The value to mutate.</param>
    /// <returns>The mutated value.</returns>
    protected abstract JSONValue EvaluateMutation(ExpressionContext context, JSONValue originalValue, JSONValue sourceData);
}
