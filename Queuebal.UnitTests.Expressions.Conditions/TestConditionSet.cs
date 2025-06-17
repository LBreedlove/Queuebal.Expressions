using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions.Conditions;


[TestClass]
public class TestConditionSet
{
    [TestMethod]
    public void test_evaluate_when_operator_is_and_and_last_condition_is_false()
    {
        var conditionSet = new ConditionSet
        {
            Conditions = [new IsNullCondition { NegateResult = true }, new IsNullCondition()],
            Operator = ConditionSetOperator.And
        };

        bool result = conditionSet.Evaluate(new ExpressionContext(new Queuebal.Json.Data.DataProvider()), new JSONValue("test"));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void test_evaluate_when_operator_is_and_and_first_condition_is_false()
    {
        var conditionSet = new ConditionSet
        {
            Conditions = [new IsNullCondition(), new IsNullCondition { NegateResult = true }],
            Operator = ConditionSetOperator.And
        };

        bool result = conditionSet.Evaluate(new ExpressionContext(new Queuebal.Json.Data.DataProvider()), new JSONValue("test"));
        Assert.IsFalse(result);
    }


    [TestMethod]
    public void test_evaluate_when_operator_is_and_and_all_conditions_are_true()
    {
        var conditionSet = new ConditionSet
        {
            Conditions = [new IsNullCondition { NegateResult = true }, new IsNullCondition { NegateResult = true }],
            Operator = ConditionSetOperator.And
        };

        bool result = conditionSet.Evaluate(new ExpressionContext(new Queuebal.Json.Data.DataProvider()), new JSONValue("test"));
        Assert.IsTrue(result);
    }


    [TestMethod]
    public void test_evaluate_when_operator_is_or_and_last_condition_is_true()
    {
        var conditionSet = new ConditionSet
        {
            Conditions = [new IsNullCondition { NegateResult = true }, new IsNullCondition()],
            Operator = ConditionSetOperator.Or
        };

        bool result = conditionSet.Evaluate(new ExpressionContext(new Queuebal.Json.Data.DataProvider()), new JSONValue());
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_operator_is_and_and_first_condition_is_true()
    {
        var conditionSet = new ConditionSet
        {
            Conditions = [new IsNullCondition(), new IsNullCondition()],
            Operator = ConditionSetOperator.Or
        };

        bool result = conditionSet.Evaluate(new ExpressionContext(new Queuebal.Json.Data.DataProvider()), new JSONValue());
        Assert.IsTrue(result);
    }


    [TestMethod]
    public void test_evaluate_when_operator_is_and_and_all_conditions_are_false()
    {
        var conditionSet = new ConditionSet
        {
            Conditions = [new IsNullCondition { NegateResult = true }, new IsNullCondition { NegateResult = true }],
            Operator = ConditionSetOperator.Or
        };

        bool result = conditionSet.Evaluate(new ExpressionContext(new Queuebal.Json.Data.DataProvider()), new JSONValue());
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void test_evaluate_when_condition_set_is_empty()
    {
        var conditionSet = new ConditionSet
        {
            Conditions = [],
            Operator = ConditionSetOperator.And
        };

        bool result = conditionSet.Evaluate(new ExpressionContext(new Queuebal.Json.Data.DataProvider()), new JSONValue("test"));
        Assert.IsTrue(result); // Default behavior is to return true when there are no conditions
    }

    [TestMethod]
    public void test_evaluate_when_condition_set_is_empty_and_negate_result_is_true()
    {
        var conditionSet = new ConditionSet
        {
            Conditions = [],
            Operator = ConditionSetOperator.And,
            NegateResult = true,
        };

        bool result = conditionSet.Evaluate(new ExpressionContext(new Queuebal.Json.Data.DataProvider()), new JSONValue("test"));
        Assert.IsFalse(result);
    }

}