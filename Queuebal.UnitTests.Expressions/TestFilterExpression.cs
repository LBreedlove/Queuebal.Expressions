using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestFilterExpression
{
    [TestMethod]
    public void test_evaluate_when_input_is_not_list_throws_exception()
    {
        var expression = new FilterExpression
        {
            Condition = new ConditionExpression
            {
                ConditionSet = new ConditionSet
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
                ConditionSet = new ConditionSet
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
}