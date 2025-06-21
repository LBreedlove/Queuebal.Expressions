using Queuebal.Expressions.Conditions;
using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;
using Queuebal.Json.Data;

using System.Text.Json;

namespace Queuebal.UnitTests.Expressions.Mutations;


[TestClass]
public class TestToDictMutation
{
    [TestMethod]
    public void test_evaluate_when_key_selector_is_null()
    {
        // Arrange
        var context = new ExpressionContext(new DataProvider());
        var mutation = new ToDictMutation
        {
            KeySelector = null,
            Condition = null
        };

        var inputValue = new JSONValue(new List<JSONValue>
        {
            new JSONValue("value1"),
            new JSONValue("value2"),
            new JSONValue("value3")
        });

        // Act
        var result = mutation.Evaluate(context, inputValue);

        // Assert
        Assert.IsTrue(result.IsDict);
        var dict = result.DictValue;

        Assert.AreEqual(3, dict.Count);
        Assert.AreEqual("value1", dict["0"].StringValue);
        Assert.AreEqual("value2", dict["1"].StringValue);
        Assert.AreEqual("value3", dict["2"].StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_null_returns_empty_dict()
    {
        // Arrange
        var context = new ExpressionContext(new DataProvider());
        var mutation = new ToDictMutation
        {
            KeySelector = null,
            Condition = null
        };
        var inputValue = new JSONValue();

        // Act
        var result = mutation.Evaluate(context, inputValue);

        // Assert
        Assert.IsTrue(result.IsDict);
        Assert.AreEqual(0, result.DictValue.Count);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_not_list_throws_exception()
    {
        // Arrange
        var context = new ExpressionContext(new DataProvider());
        var mutation = new ToDictMutation
        {
            KeySelector = null,
            Condition = null
        };

        var inputValue = new JSONValue("not a list");

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_with_key_selector_and_condition()
    {
        // Arrange
        var context = new ExpressionContext(new DataProvider());
        var mutation = new ToDictMutation
        {
            KeySelector = new DataSelectorExpression { Path = "key" },
            ValueSelector = new DataSelectorExpression { Path = "other" },
            Condition = new ConditionExpression
            {
                ConditionSet = new ConditionSet
                {
                    Conditions = new List<ICondition>
                    {
                        // only inlude entries where the key is not "value2"
                        new EqualsCondition
                        {
                            NegateResult = true,
                            ValueSelector = new DataSelectorExpression
                            {
                                Path = "value2"
                            },
                            ComparerValueExpression = new ValueExpression
                            {
                                Value = new JSONValue("data2")
                            },
                        }
                    }
                }
            }
        };

        var inputValue = """
            [
                { "key": "value1", "other": "data1" },
                { "key": "value2", "other": "data2" },
                { "key": "value3", "other": "data3" },
                { "key": "value4", "other": "data4" }
            ]
        """;

        var input = new JSONValue(JsonDocument.Parse(inputValue).RootElement);

        // Act
        var result = mutation.Evaluate(context, input);

        // Assert
        Assert.IsTrue(result.IsDict);
        var dict = result.DictValue;

        Assert.AreEqual(3, dict.Count);
        Assert.AreEqual("data1", dict["value1"].StringValue);
        Assert.AreEqual("data3", dict["value3"].StringValue);
        Assert.AreEqual("data4", dict["value4"].StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_non_string_key_selected_throws_exception()
    {
        // Arrange
        var context = new ExpressionContext(new DataProvider());
        var mutation = new ToDictMutation
        {
            KeySelector = new DataSelectorExpression { Path = "key" },
            ValueSelector = new DataSelectorExpression { Path = "value" }
        };

        var inputValue = """
            [
                { "key": 123, "value": "data1" },
                { "key": 456, "value": "data2" }
            ]
        """;

        var input = new JSONValue(JsonDocument.Parse(inputValue).RootElement);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, input));
    }
}