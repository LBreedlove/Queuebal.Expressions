using Queuebal.Json;

namespace Queuebal.Expressions;

/// <summary>
/// The base interface all conditions must implement.
/// </summary>
/// <remarks
/// This interface is to be implemented by the UnaryCondition, BinaryCondition, and ConditionSet classes.
/// It is not to be implemented by the individual condition classes like EqualsCondition. However,
/// the individual condition classes must still implement the ConditionType property, in order to be
/// deserialized correctly.
/// </remarks>
public interface ICondition
{
    /// <summary>
    /// Gets the name of the condition type.
    /// </summary>
    public static string ConditionType => throw new NotImplementedException("ConditionType must be implemented in derived classes.");

    /// <summary>
    /// Evaluates the condition and returns a boolean indicating whether the condition is met.
    /// </summary>
    bool Evaluate(ExpressionContext context, JSONValue inputValue);
}

/// <summary>
/// The base class for conditions that can be evaluated.
/// </summary>
public abstract class UnaryCondition : ICondition
{
    /// <summary>
    /// Indicates if the result of the condition should be negated.
    /// </summary>
    public bool NegateResult { get; set; } = false;

    /// <summary>
    /// Evaluates the condition and returns a boolean indicating whether the condition is met.
    /// </summary>
    /// <param name="context">The context the condition is running in.</param>
    /// <param name="inputValue">The value the condition is evaluated against.</param>
    /// <returns></returns>
    public bool Evaluate(ExpressionContext context, JSONValue inputValue)
    {
        // Evaluate the condition using the provided context and input value
        bool result = EvaluateCondition(context, inputValue);

        // If NegateResult is true, invert the result
        return NegateResult ? !result : result;
    }

    /// <summary>
    /// Method to be implemented by derived classes to evaluate the condition.
    /// </summary>
    /// <param name="context">The context the condition is running in.</param>
    /// <param name="inputValue">The value the condition is evaluated against.</param>
    /// <returns>true if the condition is met, otherwise false.</returns>
    protected abstract bool EvaluateCondition(ExpressionContext context, JSONValue inputValue);
}

/// <summary>
/// Represents a binary condition that compares an input value against a value derived from an expression.
/// </summary>
public abstract class BinaryCondition : ICondition
{
    /// <summary>
    /// Indicates if the result of the condition should be negated.
    /// </summary>
    public bool NegateResult { get; set; } = false;

    /// <summary>
    /// The expression to run to calculate the value to compare against.
    /// </summary>
    public required Expression ComparerValueExpression { get; set; }

    /// <summary>
    /// Evaluates the condition and returns a boolean indicating whether the condition is met.
    /// </summary>
    /// <param name="context">The context the condition is running in.</param>
    /// <param name="inputValue">The value the condition is evaluated against.</param>
    /// <returns></returns>
    public bool Evaluate(ExpressionContext context, JSONValue inputValue)
    {
        // get the value to compare against by evaluating the ValueExpression
        var comparerValue = ComparerValueExpression.Evaluate(context, inputValue);

        // Evaluate the condition using the provided context and input value
        bool result = EvaluateCondition(context, inputValue, comparerValue);

        // If NegateResult is true, invert the result
        return NegateResult ? !result : result;
    }

    /// <summary>
    /// Method to be implemented by derived classes to evaluate the condition.
    /// </summary>
    /// <param name="context">The context the condition is running in.</param>
    /// <param name="inputValue">The value the condition is evaluated against.</param>
    /// <returns>true if the condition is met, otherwise false.</returns>
    protected abstract bool EvaluateCondition(ExpressionContext context, JSONValue inputValue, JSONValue comparerValue);
}
