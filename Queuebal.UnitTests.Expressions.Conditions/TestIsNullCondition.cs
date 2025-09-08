using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions.Conditions;


[TestClass]
public class TestIsNullCondition
{
    [TestMethod]
    public void test_evaluate_when_value_is_null()
    {
        var condition = new IsNullCondition();
        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());
        var inputValue = new JSONValue();

        bool result = condition.Evaluate(context, inputValue);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_value_is_not_null()
    {
        var condition = new IsNullCondition();
        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());
        var inputValue = new JSONValue("test");

        bool result = condition.Evaluate(context, inputValue);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void test_evaluate_with_input_selector()
    {
        var condition = new IsNullCondition
        {
            ValueSelector = new DataSelectorExpression
            {
                Path = "testKey"
            }
        };

        var inputValue = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "testKey", new JSONValue("testValue") }
        });
        var context = new ExpressionContext(new Json.Data.VariableProvider());

        bool result = condition.Evaluate(context, inputValue);
        Assert.IsFalse(result);
    }
}
