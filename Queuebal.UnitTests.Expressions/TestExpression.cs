using Queuebal.Json;
using Queuebal.Json.Data;
using Queuebal.Expressions;
using Queuebal.Serialization;

namespace Queuebal.UnitTests.Expressions;


public class InvalidExpression : Expression
{
    /// <summary>
    /// Evaluates the expression and returns the result.
    /// </summary>
    /// <param name="context">The context the expression is running in.</param>
    /// <param name="inputValue">The inputValue for the expression.</param>
    /// <returns>The value defined by the expression.</returns>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        return inputValue;
    }

    // this expression will throw an exception when we attempt to register it, because it does not implement the ExpressionType property
}


[TestClass]
public class TestExpression
{
    [TestMethod]
    public void test_register_expression_without_expression_type_throws_exception()
    {
        // Arrange
        var expressionRegistry = new TypeRegistryService<IExpression>("ExpressionType");

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expressionRegistry.RegisterTypeMapping(typeof(InvalidExpression)));
    }

    [TestMethod]
    public void test_evaluate_invalid_expression_returns_input_value()
    {
        // This test is here just so our test file doesn't have incomplete coverage. :facepalm:

        // Arrange
        var expression = new InvalidExpression();
        var inputJson = """
        {
            "key": "value"
        }
        """;
        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);
        var context = new ExpressionContext(new DataProvider());

        // Act
        var result = expression.Evaluate(context, inputValue);

        // Assert
        Assert.AreEqual(inputValue, result);
    }
}
