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
    public void test_evaluate_when_lvalue_is_not_number_or_string()
    {
        // Arrange
        var expression = new MinExpression
        {
            LValue = new ValueExpression { Value = new List<JSONValue> { 123 } },
            RValue = new ValueExpression { Value = 123 }
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(Context, new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_rvalue_is_not_number_or_string()
    {
        // Arrange
        var expression = new MinExpression
        {
            LValue = new ValueExpression { Value = 123 },
            RValue = new ValueExpression { Value = new List<JSONValue> { 123 } }
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
            LValue = new ValueExpression { Value = 123 },
            RValue = new ValueExpression { Value = "test" }
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(Context, new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_lvalue_is_string_and_rvalue_is_number()
    {
        // Arrange
        var expression = new MinExpression
        {
            LValue = new ValueExpression { Value = "test" },
            RValue = new ValueExpression { Value = 123 }
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
            LValue = new ValueExpression { Value = 456 },
            RValue = new ValueExpression { Value = 123 }
        };

        // Act
        var result = expression.Evaluate(Context, new JSONValue());

        // Assert
        Assert.AreEqual((double)123, result.FloatValue);
    }


    [TestMethod]
    public void test_evaluate_when_both_values_are_strings_lvalue_is_less()
    {
        // Arrange
        var expression = new MinExpression
        {
            LValue = new ValueExpression { Value = "hello" },
            RValue = new ValueExpression { Value = "world" }
        };

        // Act
        var result = expression.Evaluate(Context, new JSONValue());

        // Assert
        Assert.AreEqual("hello", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_both_values_are_strings_rvalue_is_less()
    {
        // Arrange
        var expression = new MinExpression
        {
            LValue = new ValueExpression { Value = "world" },
            RValue = new ValueExpression { Value = "hello" }
        };

        // Act
        var result = expression.Evaluate(Context, new JSONValue());

        // Assert
        Assert.AreEqual("hello", result.StringValue);
    }
}