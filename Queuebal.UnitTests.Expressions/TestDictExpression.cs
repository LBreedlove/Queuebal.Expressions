using Queuebal.Expressions;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestDictExpression
{
    [TestMethod]
    public void test_evaluate_when_key_is_not_string_throws_exception()
    {
        // Arrange
        var expression = new DictExpression
        {
            Value = new Dictionary<string, IExpression>
            {
                // this key will be replaced by the VariableReplacement with a non-string value
                { "${replace}", new ValueExpression { Value = new JSONValue("value") } },
            }
        };

        var variableProvider = new Json.Data.VariableProvider();
        variableProvider.AddValue("replace", new JSONValue(123)); // Non-string value for key

        var context = new ExpressionContext(variableProvider);
        var inputValue = new JSONValue("not used");

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_valid_dict()
    {
        var expression = new DictExpression
        {
            Value = new Dictionary<string, IExpression>
            {
                { "key1", new ValueExpression { Value = new JSONValue("value1") } },
                { "key2", new ValueExpression { Value = new JSONValue("value2") } }
            }
        };

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue("not used");

        // Act
        var result = expression.Evaluate(context, inputValue);

        // Assert
        Assert.IsTrue(result.IsDict);
        Assert.AreEqual(2, result.DictValue.Count);

        Assert.IsTrue(result.DictValue.ContainsKey("key1"));
        Assert.IsTrue(result.DictValue.ContainsKey("key2"));

        Assert.AreEqual("value1", result.DictValue["key1"].StringValue);
        Assert.AreEqual("value2", result.DictValue["key2"].StringValue);
    }
}
