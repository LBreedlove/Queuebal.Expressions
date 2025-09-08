using Queuebal.Expressions;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions;


[TestClass]
public class TestMapExpression
{
    [TestMethod]
    public void test_evaluate_when_input_is_not_list_throws_exception()
    {
        var expression = new MapExpression
        {
            Map = new ValueExpression { Value = new JSONValue("test") }
        };

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue("not a list");

        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_is_list_returns_mapped_values()
    {
        var expression = new MapExpression
        {
            Map = new ValueExpression { Value = new JSONValue("mapped") }
        };

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new List<JSONValue> { new JSONValue("item1"), new JSONValue("item2") };
        var result = expression.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsList);
        Assert.AreEqual(2, result.ListValue.Count);
        Assert.AreEqual("mapped", result.ListValue[0].StringValue);
        Assert.AreEqual("mapped", result.ListValue[1].StringValue);
    }
}