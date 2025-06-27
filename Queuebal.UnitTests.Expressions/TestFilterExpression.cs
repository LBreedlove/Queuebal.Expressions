using System.Collections.ObjectModel;
using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestFilterExpression
{
    [TestMethod]
    public void test_evaluate_when_input_is_not_list_or_dict_throws_exception()
    {
        var expression = new FilterExpression
        {
            Condition = new ConditionExpression
            {
                Condition = new ConditionSet
                {
                    Conditions = [
                        new EqualsCondition
                        {
                            ComparerValue = new ValueExpression { Value = new JSONValue("test") }
                        }
                    ]
                }
            }
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());
        var inputValue = new JSONValue("not a list");

        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_condition_is_true_includes_item_in_results()
    {
        var expression = new FilterExpression
        {
            Condition = new ConditionExpression
            {
                Condition = new ConditionSet
                {
                    Conditions = [
                        new EqualsCondition
                        {
                            ComparerValue = new ValueExpression { Value = new JSONValue("test") }
                        }
                    ]
                }
            }
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());
        var inputValue = new List<JSONValue> { new JSONValue("test"), new JSONValue("not included") };
        var result = expression.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsList);
        Assert.AreEqual(1, result.ListValue.Count);
        Assert.AreEqual("test", result.ListValue[0].StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_value_is_dict()
    {
        var inputValue = new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", 123 },
            {
                "key3", new List<JSONValue>
                {
                    456,
                    "hello world",
                }
            },
            { "key4", new JSONValue() },
        };

        var condition = new ConditionExpression
        {
            Condition = new IsNullCondition { NegateResult = true, ValueSelector = new DataSelectorExpression { Path = "value" } }
        };

        var expression = new FilterExpression { Condition = condition };
        var result = expression.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), inputValue);
        Assert.IsNotNull(result);

        var expected = new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", 123 },
            {
                "key3", new List<JSONValue>
                {
                    456,
                    "hello world",
                }
            },
        };
        CollectionAssert.AreEqual(expected, result.DictValue);
    }
}