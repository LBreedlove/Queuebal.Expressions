using Queuebal.Json;
using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;

namespace Queuebal.UnitTests.Expressions.Conditions;


[TestClass]
public class TestEqualsCondition
{
    [TestMethod]
    public void test_evaluate_when_values_are_equal()
    {
        var condition = new EqualsCondition
        {
            ComparerValueExpression = new ValueExpression { Value = new("test") }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());
        var inputValue = new JSONValue("test");

        bool result = condition.Evaluate(context, inputValue);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_values_are_not_equal()
    {
        var condition = new EqualsCondition
        {
            ComparerValueExpression = new ValueExpression { Value = new("test") }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());
        var inputValue = new JSONValue("testing");

        bool result = condition.Evaluate(context, inputValue);
        Assert.IsFalse(result);
    }


    [TestMethod]
    public void test_evaluate_when_values_are_not_equal_and_negate_result()
    {
        var condition = new EqualsCondition
        {
            NegateResult = true,
            ComparerValueExpression = new ValueExpression { Value = new("test") }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());
        var inputValue = new JSONValue("testing");

        bool result = condition.Evaluate(context, inputValue);
        Assert.IsTrue(result);
    }
}
