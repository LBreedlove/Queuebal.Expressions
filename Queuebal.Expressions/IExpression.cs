using System.Runtime;
using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.Expressions;

/// <summary>
/// Represents an evaluable expression.
/// </summary>
public interface IExpression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// </summary>
    public static string ExpressionType => throw new NotImplementedException("ExpressionType must be implemented in derived classes.");

    /// <summary>
    /// Evaluates the expression and returns the result.
    /// </summary>
    JSONValue Evaluate(ExpressionContext context, JSONValue inputValue);
}

/// <summary>
/// Represents an abstract base class for expressions that can be evaluated.
/// </summary>
public abstract class Expression : IExpression
{
    /// <summary>
    /// The expression to run to calculate the input value for this expression.
    /// This property is used to support expression chaining.
    /// </summary>
    public Expression? InputValue { get; set; }

    /// <summary>
    /// Evaluates the expression and returns the result.
    /// </summary>
    /// <param name="context">The context the expression is being evaluated in.</param>
    /// <param name="inputValue">The inputValue to the expression.</param>
    /// <returns>A JSONValue containing the result of the expression evaluation.</returns>
    public JSONValue Evaluate(ExpressionContext context, JSONValue inputValue)
    {
        // If there is an InputValue, evaluate it first - this allows for expressions chaining
        if (InputValue != null)
        {
            inputValue = InputValue.Evaluate(context, inputValue);
        }

        // if the inputValue is a string, we need to tokenize it and see if it
        // represents a variable value in the context's data provider
        if (inputValue.IsString)
        {
            inputValue = Tokenizer.Evaluate(inputValue.StringValue, context.DataProvider);
        }

        // Evaluate the expression with the provided context and input value
        var result = EvaluateExpression(context, inputValue);
        if (result.IsString)
        {
            result = Tokenizer.Evaluate(result.StringValue, context.DataProvider);
        }

        return result;
    }

    /// <summary>
    /// Evaluates the expression and returns the result.
    /// </summary>
    protected abstract JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue);
}
