using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.Expressions;

/// <summary>
/// Indicates how the results of the conditions in a ConditionSet should be combined.
/// </summary>
public enum ConditionSetOperator
{
    And,
    Or
}

/// <summary>
/// Represents a set of conditions that can be evaluated together.
/// </summary>
public class ConditionSet : ICondition
{
    /// <summary>
    /// Gets the name of the condition type.
    /// </summary>
    public static string ConditionType { get; } = "ConditionSet";

    /// <summary>
    /// Gets a ConditionSet that always evaluates to true.
    /// </summary>
    public static ConditionSet AlwaysTrue => new ConditionSet
    {
        NegateResult = false,
        Operator = ConditionSetOperator.And,
        Conditions = new List<ICondition>()
    };

    /// <summary>
    /// Gets a ConditionSet that always evaluates to false.
    /// </summary>
    public static ConditionSet AlwaysFalse => new ConditionSet
    {
        NegateResult = true,
        Operator = ConditionSetOperator.And,
        Conditions = new List<ICondition>()
    };

    /// <summary>
    /// Indicates if the result of the condition set should be negated.
    /// </summary>
    public bool NegateResult { get; set; } = false;

    /// <summary>
    /// Indicates how the results of the conditions in this set should be combined.
    /// </summary>
    public ConditionSetOperator Operator { get; set; } = ConditionSetOperator.And;

    /// <summary>
    /// The conditions in this set.
    /// </summary>
    public required List<ICondition> Conditions { get; set; }

    /// <summary>
    /// An optional value selector expression that can be used to select a value from the input
    /// value to be used in the evaluation of the conditions.
    /// This is useful when the conditions need to be evaluated against a specific part of the input
    /// value, rather than the entire input value.
    /// If this is not set, the conditions will be evaluated against the entire input value.
    /// </summary>
    public IExpression? ValueSelector { get; set; }

    /// <summary>
    /// Evaluates the conditions in the set, combining the results based on the specified operator
    /// and returns the result as a boolean.
    /// </summary>
    /// <param name="context">The context the ConditionSet is running in.</param>
    /// <param name="inputValue">The value the ConditionSet is evaluated against.</param>
    /// <returns>
    /// If the Operator is And, and all the conditions evaluate to true, returns true.
    /// If the Operator is And, and any condition evaluates to false, returns false.
    /// If the Operator is Or, and any condition evaluates to true, returns true.
    /// If the Operator is Or, and all conditions evaluate to false, returns false.
    /// If there are no conditions, returns true by default.
    /// If any condition is null, throws an InvalidOperationException.
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public bool Evaluate(ExpressionContext context, JSONValue inputValue)
    {
        bool? result = null;
        if (ValueSelector != null)
        {
            // If a ValueSelector is provided, evaluate it to get the value to use for the conditions.
            inputValue = ValueSelector.Evaluate(context, inputValue);
        }

        foreach (var condition in Conditions)
        {
            var conditionResult = condition.Evaluate(context, inputValue);
            if (Operator == ConditionSetOperator.And)
            {
                if (!conditionResult)
                {
                    result = false;
                    break;
                }

                result = true;
            }
            else
            {
                if (conditionResult)
                {
                    result = true;
                    break;
                }

                result = false;
            }
        }

        if (result == null)
        {
            // if there are no conditions, we consider the result to be true
            result = true;
        }

        return NegateResult ? !result.Value : result.Value;
    }
}
