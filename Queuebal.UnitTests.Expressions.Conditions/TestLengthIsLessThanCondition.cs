using Queuebal.Json;
using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;

namespace Queuebal.UnitTests.Expressions.Conditions;


[TestClass]
public class TestLengthIsLessThanCondition
{
    ExpressionContext Context { get; } = new ExpressionContext(new Json.Data.DataProvider());

    [TestMethod]
    public void test_evaluate_when_input_is_null_returns_false()
    {
        // Arrange
        var inputValue = new JSONValue();
        var comparer = new ValueExpression { Value = "hello" };

        // Act
        var result = new LengthIsLessThanCondition { ComparerValue = comparer }.Evaluate(Context, inputValue);

        // Assert
        Assert.IsFalse(result);  // compare to null always returns false
    }

    [TestMethod]
    public void test_evaluate_when_input_is_invalid_type_throws()
    {
        // Arrange
        var inputValue = true;
        var comparer = new ValueExpression { Value = "hello" };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => new LengthIsLessThanCondition { ComparerValue = comparer }.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_and_comparer_is_number()
    {
        // Arrange
        var inputValue = "hello";
        var comparer = new ValueExpression { Value = 6 };

        // Act
        var result = new LengthIsLessThanCondition { ComparerValue = comparer }.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_list()
    {
        // Arrange
        var inputValue = new List<JSONValue> { "hello" };
        var comparer = new ValueExpression { Value = new List<JSONValue>{ "hello", "world" } };

        // Act
        var result = new LengthIsLessThanCondition { ComparerValue = comparer }.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_dict()
    {
        // Arrange
        var inputValue = new Dictionary<string, JSONValue>
        {
            { "hello", "world" },
            { "goodbye", "moon" },
        };

        var comparer = new ValueExpression
        {
            Value = new Dictionary<string, JSONValue>
            {
                { "hello", "world" },
            }
        };

        // Act
        var result = new LengthIsLessThanCondition { ComparerValue = comparer }.Evaluate(Context, inputValue);

        // Assert
        Assert.IsFalse(result);
    }
}
