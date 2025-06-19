using Queuebal.Expressions;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestListExpression
{
    [TestMethod]
    public void test_evaluate_when_list_is_empty_returns_empty_list()
    {
        // Arrange
        var expression = new ListExpression
        {
            Value = new List<IExpression>()
        };

        // Act
        var result = expression.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue("not used"));

        // Assert
        Assert.IsTrue(result.IsList);
        Assert.AreEqual(0, result.ListValue.Count);
    }

    [TestMethod]
    public void test_evaluate_when_list_contains_values_returns_list()
    {
        // Arrange
        var expression = new ListExpression
        {
            Value = new List<IExpression>
            {
                new ValueExpression { Value = new JSONValue("value1") },
                new ValueExpression { Value = new JSONValue("value2") }
            }
        };

        // Act
        var result = expression.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue("not used"));

        // Assert
        Assert.IsTrue(result.IsList);
        Assert.AreEqual(2, result.ListValue.Count);
        Assert.AreEqual("value1", result.ListValue[0].StringValue);
        Assert.AreEqual("value2", result.ListValue[1].StringValue);
    }
}
