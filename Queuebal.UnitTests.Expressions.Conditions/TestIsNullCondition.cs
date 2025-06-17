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
        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());
        var inputValue = new JSONValue();

        bool result = condition.Evaluate(context, inputValue);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_value_is_not_null()
    {
        var condition = new IsNullCondition();
        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());
        var inputValue = new JSONValue("test");

        bool result = condition.Evaluate(context, inputValue);
        Assert.IsFalse(result);
    }
}
