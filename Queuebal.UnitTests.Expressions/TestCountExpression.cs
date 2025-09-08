using Queuebal.Json;
using Queuebal.Json.Data;
using Queuebal.Expressions;

namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestCountExpression
{
    public TestCountExpression()
    {
        Context = new ExpressionContext(new VariableProvider());
    }

    /// <summary>
    /// The ExpressionContext used by the tests.
    /// </summary>
    protected ExpressionContext Context { get; }

    [TestMethod]
    public void test_evaluate_when_non_container_type_provided_throws()
    {
        // Arrange
        var inputValue = new JSONValue();
        var value = new ValueExpression { Value = 123 };
        var expression = new CountExpression { Value = value };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_non_container_type_provided_and_exclude_falsey_throws()
    {
        // Arrange
        var inputValue = new JSONValue();
        var value = new ValueExpression { Value = 0 };
        var expression = new CountExpression { ExcludeFalsey = true, Value = value };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_string_value_provided_returns_length()
    {
        // Arrange
        var inputValue = new JSONValue();
        var value = new ValueExpression { Value = "test value" };
        var expression = new CountExpression { Value = value };

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(value.Value.StringValue.Length, result);
    }

    [TestMethod]
    public void test_evaluate_when_string_value_provided_and_exclude_falsey_returns_length()
    {
        // Arrange
        var inputValue = new JSONValue();
        var value = new ValueExpression { Value = "test value" };
        var expression = new CountExpression { ExcludeFalsey = true, Value = value };

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(value.Value.StringValue.Length, result);
    }

    [TestMethod]
    public void test_evaluate_when_list_value_provided_returns_length()
    {
        // Arrange
        var inputValue = new JSONValue();
        var value = new ValueExpression {
            Value = new List<JSONValue>
            {
                123,
                false,
                0,
                new Dictionary<string, JSONValue>
                {
                    { "key", new JSONValue() },
                },
                "test value",
                new List<JSONValue>(),
                new List<JSONValue>
                {
                    "test value",
                }
            }
        };

        var expression = new CountExpression { Value = value };

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(value.Value.ListValue.Count, result);
    }

    [TestMethod]
    public void test_evaluate_when_list_value_provided_and_exclude_falsey_returns_length_excluding_falsey_items()
    {
        // Arrange
        var inputValue = new JSONValue();
        var value = new ValueExpression
        {
            Value = new List<JSONValue>
            {
                123,
                false,
                0,
                new Dictionary<string, JSONValue>
                {
                    { "key", new JSONValue() },
                },
                "test value",
                new List<JSONValue>(),
                new List<JSONValue>
                {
                    "test value",
                },
                new Dictionary<string, JSONValue>(),
            }
        };

        var expression = new CountExpression { ExcludeFalsey = true, Value = value };

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(4, result);
    }

    [TestMethod]
    public void test_evaluate_when_dict_value_provided_returns_length()
    {
        // Arrange
        var inputValue = new JSONValue();
        var value = new ValueExpression
        {
            Value = new Dictionary<string, JSONValue>
            {
                { "1", 123 },
                { "2", false },
                { "3", 0 },
                { "4", new Dictionary<string, JSONValue>
                {
                    { "key", new JSONValue() },
                } },
                { "5", "test value" },
                { "6", new List<JSONValue>() },
                { "7", new List<JSONValue>
                {
                    "test value",
                } }
            }
        };

        var expression = new CountExpression { Value = value };

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(value.Value.DictValue.Count, result);
    }

    [TestMethod]
    public void test_evaluate_when_dict_value_provided_and_exclude_falsey_returns_length_excluding_falsey_items()
    {
        // Arrange
        var inputValue = new JSONValue();
        var value = new ValueExpression
        {
            Value = new Dictionary<string, JSONValue>
            {
                { "1", 123 },
                { "2", false },
                { "3", 0 },
                { "4", new Dictionary<string, JSONValue>
                {
                    { "key", new JSONValue() },
                } },
                { "5", "test value" },
                { "6", new List<JSONValue>() },
                { "7", new List<JSONValue>
                {
                    "test value",
                } }
            }
        };

        var expression = new CountExpression { ExcludeFalsey = true, Value = value };

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(4, result);
    }
}