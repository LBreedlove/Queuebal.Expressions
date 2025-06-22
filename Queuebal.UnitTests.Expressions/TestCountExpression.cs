using Queuebal.Json;
using Queuebal.Json.Data;
using Queuebal.Expressions;

namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestCountExpression
{
    public TestCountExpression()
    {
        Context = new ExpressionContext(new DataProvider());
    }

    /// <summary>
    /// The ExpressionContext used by the tests.
    /// </summary>
    protected ExpressionContext Context { get; }

    [TestMethod]
    public void test_evaluate_when_non_container_type_provided_throws()
    {
        // Arrange
        var inputValue = 123;
        var expression = new CountExpression();

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_non_container_type_provided_and_exclude_falsey_throws()
    {
        // Arrange
        var inputValue = 0;
        var expression = new CountExpression { ExcludeFalsey = true };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_string_value_provided_returns_length()
    {
        // Arrange
        var inputValue = "test value";
        var expression = new CountExpression();

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(inputValue.Length, result);
    }

    [TestMethod]
    public void test_evaluate_when_string_value_provided_and_exclude_falsey_returns_length()
    {
        // Arrange
        var inputValue = "test value";
        var expression = new CountExpression { ExcludeFalsey = true };

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(inputValue.Length, result);
    }

    [TestMethod]
    public void test_evaluate_when_list_value_provided_returns_length()
    {
        // Arrange
        var inputValue = new List<JSONValue>
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
        };

        var expression = new CountExpression();

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(inputValue.Count, result);
    }

    [TestMethod]
    public void test_evaluate_when_list_value_provided_and_exclude_falsey_returns_length_excluding_falsey_items()
    {
        // Arrange
        var inputValue = new List<JSONValue>
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
        };

        var expression = new CountExpression { ExcludeFalsey = true };

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(4, result);
    }

    [TestMethod]
    public void test_evaluate_when_dict_value_provided_returns_length()
    {
        // Arrange
        var inputValue = new Dictionary<string, JSONValue>
        {
            {"1", 123},
            {"2", false},
            {"3", 0},
            {"4", new Dictionary<string, JSONValue>
            {
                { "key", new JSONValue() },
            }},
            {"5", "test value"},
            {"6", new List<JSONValue>()},
            {"7", new List<JSONValue>
            {
                "test value",
            }}
        };

        var expression = new CountExpression();

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(inputValue.Count, result);
    }

    [TestMethod]
    public void test_evaluate_when_dict_value_provided_and_exclude_falsey_returns_length_excluding_falsey_items()
    {
        // Arrange
        var inputValue = new Dictionary<string, JSONValue>
        {
            {"1", 123},
            {"2", false},
            {"3", 0},
            {"4", new Dictionary<string, JSONValue>
            {
                { "key", new JSONValue() },
            }},
            {"5", "test value"},
            {"6", new List<JSONValue>()},
            {"7", new List<JSONValue>
            {
                "test value",
            }}
        };

        var expression = new CountExpression { ExcludeFalsey = true };

        // Act
        var result = expression.Evaluate(Context, inputValue).IntValue;

        // Assert
        Assert.AreEqual(4, result);
    }
}