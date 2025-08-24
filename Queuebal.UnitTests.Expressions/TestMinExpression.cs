using Queuebal.Expressions;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestMinExpression
{
    public TestMinExpression()
    {
        Context = new ExpressionContext(new Json.Data.DataProvider());
    }

    /// <summary>
    /// The context the expressions are evaluated in.
    /// </summary>
    public ExpressionContext Context { get; }

    [TestMethod]
    public void test_evaluate_when_values_is_not_list()
    {
        // Arrange
        var expression = new MinExpression
        {
            Values = new ValueExpression { Value = 123 },
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(Context, new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_lvalue_is_not_number_or_string()
    {
        // Arrange
        var expression = new MinExpression
        {
            Values = new ValueExpression { Value = new List<JSONValue> { new List<JSONValue> { 123 } } },
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(Context, new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_lvalue_is_number_and_rvalue_is_string()
    {
        // Arrange
        var expression = new MinExpression
        {
            Values = new ValueExpression { Value = new List<JSONValue> { 123, "test" } },
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(Context, new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_both_values_are_numbers()
    {
        // Arrange
        var expression = new MinExpression
        {
            Values = new ValueExpression { Value = new List<JSONValue> { 456, 123 } },
        };

        // Act
        var result = expression.Evaluate(Context, new JSONValue());

        // Assert
        Assert.AreEqual((double)123, result.FloatValue);
    }


    [TestMethod]
    public void test_evaluate_when_both_values_are_strings_rvalue_is_greater()
    {
        // Arrange
        var expression = new MinExpression
        {
            Values = new ValueExpression { Value = new List<JSONValue>{ "hello", "world" } },
        };

        // Act
        var result = expression.Evaluate(Context, new JSONValue());

        // Assert
        Assert.AreEqual("hello", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_both_values_are_strings_lvalue_is_greater()
    {
        // Arrange
        var expression = new MinExpression
        {
            Values = new ValueExpression { Value = new List<JSONValue>{ "world", "hello" } },
        };

        // Act
        var result = expression.Evaluate(Context, new JSONValue());

        // Assert
        Assert.AreEqual("hello", result.StringValue);
    }
}