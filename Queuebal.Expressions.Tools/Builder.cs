using Queuebal.Expressions.Conditions;
using Queuebal.Expressions.Mutations;

namespace Queuebal.Expressions.Tools;

#pragma warning disable CS8981, IDE1006

/// <summary>
/// A class that contains builder methods for building expression trees.
/// </summary>
/// <remarks>
/// You can import this class as 'q', and reference its members succinctly,
/// e.g.
/// ```
/// using q = Queuebal.Builder;
/// var expr = q.val("hello");
/// ```
public static class Builder
{
    /// <summary>
    /// Builds a ValueExpression with the provided value.
    /// </summary>
    /// <param name="value">The value to populate in the ValueExpression.</param>
    /// <returns>A new ValueExpression containing the given value.</returns>
    public static IExpression val(string value) => new ValueExpression { Value = value };

    /// <summary>
    /// Builds a ValueExpression with the provided value.
    /// </summary>
    /// <param name="value">The value to populate in the ValueExpression.</param>
    /// <returns>A new ValueExpression containing the given value.</returns>
    public static IExpression val(int value) => new ValueExpression { Value = value };

    /// <summary>
    /// Builds a ValueExpression with the provided value.
    /// </summary>
    /// <param name="value">The value to populate in the ValueExpression.</param>
    /// <returns>A new ValueExpression containing the given value.</returns>
    public static IExpression val(float value) => new ValueExpression { Value = value };

    /// <summary>
    /// Builds a ValueExpression with the provided value.
    /// </summary>
    /// <param name="value">The value to populate in the ValueExpression.</param>
    /// <returns>A new ValueExpression containing the given value.</returns>
    public static IExpression val(double value) => new ValueExpression { Value = value };

    /// <summary>
    /// Builds a ValueExpression with the provided value.
    /// </summary>
    /// <param name="value">The value to populate in the ValueExpression.</param>
    /// <returns>A new ValueExpression containing the given value.</returns>
    public static IExpression val(bool value) => new ValueExpression { Value = value };

    /// <summary>
    /// Builds a DataSelectorExpression with the provided path.
    /// </summary>
    /// <param name="path">The path the data selector will select from.</param>
    /// <returns>A new DataSelectorExpression with the specified path.</returns>
    public static DataSelectorExpression ds(string path, IExpression? inputValue = null) =>
        new DataSelectorExpression
        {
            Path = path,
            InputValue = inputValue,
        };

    /// <summary>
    /// Represents a key value pair, for use when building a DictExpression.
    /// </summary>
    public class kv
    {
        public required string k { get; set; }
        public required IExpression v { get; set; }
    }

    /// <summary>
    /// Builds a DictExpression using the key value pairs passed as args.
    /// </summary>
    /// <param name="keyValues">The key value pairs to use to build the DictExpression value.</param>
    /// <returns>A new DictExpression containing the provided keyvalue pairs.</returns>
    public static DictExpression dict(params kv[] keyValues) =>
        new DictExpression
        {
            Value = keyValues.ToDictionary(kvp => kvp.k, kvp => kvp.v)
        };

    public static DictExpression dict_with_input(IExpression inputValue, params kv[] keyValues) =>
        new DictExpression
        {
            InputValue = inputValue,
            Value = keyValues.ToDictionary(kvp => kvp.k, kvp => kvp.v)
        };

    /// <summary>
    /// Builds a ListExpression using the values provided as args.
    /// </summary>
    /// <param name="expressions">The expressions to include in the list value.</param>
    /// <returns>A new ListExpression containing the provided expressions.</returns>
    public static ListExpression list(params IExpression[] expressions) =>
        new ListExpression
        {
            Value = expressions.ToList()
        };

    public static ListExpression list_with_input(IExpression inputValue, params IExpression[] expressions) =>
        new ListExpression
        {
            InputValue = inputValue,
            Value = expressions.ToList(),
        };

    /// <summary>
    /// Builds a FilterExpression using the provided condition and input value. 
    /// </summary>
    /// <param name="condition">The condition that determines if an input value is included in the output.</param>
    /// <param name="inputValues">A list expression containing the values to filter.</param>
    /// <returns>A new filter expression.</returns>
    public static FilterExpression filter(ConditionExpression condition, IExpression? inputValues = null) =>
        new FilterExpression
        {
            InputValue = inputValues,
            Condition = condition,
        };

    /// <summary>
    /// Represents a key value pair, for use when building a DynamicDictExpression.
    /// </summary>
    public class dyn_kv
    {
        public required IExpression k { get; set; }
        public required IExpression v { get; set; }
        public ConditionExpression? c { get; set; }
    }

    /// <summary>
    /// Builds a DynamicDictExpression using the given key value pairs as the entries in the dict.
    /// </summary>
    /// <param name="keyValues">The key value pairs to include in the DynamicDictExpression Entries.</param>
    /// <returns>A new DynamicDictExpression containing the given key value pair entries.</returns>
    public static DynamicDictExpression dyn_dict(params dyn_kv[] keyValues) => new DynamicDictExpression
    {
        Entries = keyValues.Select
        (
            kvp => new DynamicDictEntry { Key = kvp.k, Value = kvp.v, Condition = kvp.c }
        ).ToList()
    };

    /// <summary>
    /// Builds a DynamicDictExpression using the given key value pairs as the entries in the dict.
    /// </summary>
    /// <param name="keyValues">The key value pairs to include in the DynamicDictExpression Entries.</param>
    /// <returns>A new DynamicDictExpression containing the given key value pair entries.</returns>
    public static DynamicDictExpression dyn_dict_with_input(IExpression inputValue, params dyn_kv[] keyValues) => new DynamicDictExpression
    {
        InputValue = inputValue,
        Entries = keyValues.Select
        (
            kvp => new DynamicDictEntry { Key = kvp.k, Value = kvp.v, Condition = kvp.c }
        ).ToList()
    };

    /// <summary>
    /// A class used to build condition expressions.
    /// </summary>
    public static class Conditions
    {
        /// <summary>
        /// Builds a ConditionExpression containing the provided ConditionSet.
        /// </summary>
        /// <param name="conditionSet">The condition set to wrap in an expression.</param>
        /// <returns>A new ConditionExpression containing the given condition set.</returns>
        public static ConditionExpression exp(ICondition condition) =>
            new ConditionExpression
            {
                Condition = condition
            };

        /// <summary>
        /// Builds a Condition that evaluates to true when all of the contained
        /// conditions evaluate to true.
        /// </summary>
        /// <param name="conditions">The conditions within the expression's condition set.</param>
        /// <returns>true if all the conditions evaluate to true, otherwise false.</returns>
        public static ICondition all(params ICondition[] conditions) =>
            new ConditionSet
            {
                Conditions = conditions.ToList(),
                Operator = ConditionSetOperator.And,
            };

        /// <summary>
        /// Builds a Condition that evaluates to true when any of the contained
        /// conditions evaluate to true.
        /// </summary>
        /// <param name="conditions">The conditions within the expression's condition set.</param>
        /// <returns>true if any of the conditions evaluate to true, otherwise false.</returns>
        public static ICondition any(params ICondition[] conditions) =>
            new ConditionSet
            {
                Conditions = conditions.ToList(),
                Operator = ConditionSetOperator.Or,
            };

        /// <summary>
        /// Builds a Condition that evaluates to true when none of the contained
        /// conditions evaluate to true.
        /// </summary>
        /// <param name="conditions">The conditions within the expression's condition set.</param>
        /// <returns>true if none the conditions evaluate to true, otherwise false.</returns>
        public static ICondition none(params ICondition[] conditions) =>
            new ConditionSet
            {
                NegateResult = true,
                Conditions = conditions.ToList(),
                Operator = ConditionSetOperator.Or,
            };

        /// <summary>
        /// Builds an EqualsExpression.
        /// </summary>
        /// <param name="valueSelector">The expression used to select/build the value to compare to the ComparerValue.</param>
        /// <param name="comparer">The expression used to select/build the value to compare to the input value.</param>
        /// <returns>A new ICondition representing the EqualsCondition.</returns>
        public static ICondition eq(IExpression comparer, IExpression? valueSelector = null) =>
            new EqualsCondition
            {
                ValueSelector = valueSelector,
                ComparerValue = comparer
            };

        /// <summary>
        /// Builds a not EqualsExpression.
        /// </summary>
        /// <param name="valueSelector">The expression used to select/build the value to compare to the ComparerValue.</param>
        /// <param name="comparer">The expression used to select/build the value to compare to the input value.</param>
        /// <returns>A new ICondition representing the not EqualsCondition.</returns>
        public static ICondition not_eq(IExpression comparer, IExpression? valueSelector = null) =>
            new EqualsCondition
            {
                NegateResult = true,
                ValueSelector = valueSelector,
                ComparerValue = comparer
            };

        /// <summary>
        /// Builds an IsNullCondition.
        /// </summary>
        /// <param name="valueSelector">The expression used to select/build the value to check for null.</param>
        /// <returns>A new IsNullCondition.</returns>
        public static ICondition is_null(IExpression? valueSelector = null) =>
            new IsNullCondition
            {
                ValueSelector = valueSelector
            };

        /// <summary>
        /// Builds a not IsNullCondition.
        /// </summary>
        /// <param name="valueSelector">The expression used to select/build the value to check for null.</param>
        /// <returns>A new not IsNullCondition.</returns>
        public static ICondition not_null(IExpression valueSelector) =>
            new IsNullCondition
            {
                NegateResult = true,
                ValueSelector = valueSelector
            };

        // TODO: Add other Conditions
    }

    /// <summary>
    /// A class used to build mutation expressions.
    /// </summary>
    public static class Mutations
    {
        /// <summary>
        /// Builds a StringSplitMutation.
        /// </summary>
        /// <param name="inputValue">The value to split.</param>
        /// <param name="separators">The separators used to split the string.</param>
        /// <returns>An IExpression representing the StringSplitMutation.</returns>
        public static IExpression split(IExpression inputValue, params string[] separators) =>
            new MutationExpression
            {
                Mutation = new StringSplitMutation
                {
                    Separators = separators.ToList(),
                    InputValue = inputValue,
                }
            };

        /// <summary>
        /// Builds a StringJoinMutation.
        /// </summary>
        /// <param name="separator">The separator used to join the strings.</param>
        /// <param name="values">The expressions used to select/build the strings to join.</param>
        /// <returns>An IMutation representing the StringJoinMutation.</returns>
        public static IExpression join(string separator, params IExpression[] values) =>
            new MutationExpression
            {
                Mutation = new StringJoinMutation
                {
                    Separator = separator,
                    InputValue = new ListExpression
                    {
                        Value = values.ToList()
                    }
                }
            };

        // TODO: Add other Mutations
    }
}

#pragma warning restore CS8981, IDE1006
