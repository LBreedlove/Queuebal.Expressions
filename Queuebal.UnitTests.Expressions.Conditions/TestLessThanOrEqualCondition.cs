using Queuebal.Json;
using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;

namespace Queuebal.UnitTests.Expressions.Conditions;


[TestClass]
public class TestLessThanOrEqualCondition
{
    ExpressionContext Context { get; } = new ExpressionContext(new Json.Data.VariableProvider());

    [TestMethod]
    public void test_evaluate_when_input_value_is_greater_than_comparer()
    {
        // Arrange
        var inputValue = 1024;
        var comparerValue = new ValueExpression { Value = 512 };

        // Act
        var result = new LessThanOrEqualCondition { ComparerValue = comparerValue }.Evaluate(Context, inputValue);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void test_evaluate_when_input_value_is_less_than_comparer()
    {
        // Arrange
        var inputValue = 512;
        var comparerValue = new ValueExpression { Value = 1024 };

        // Act
        var result = new LessThanOrEqualCondition { ComparerValue = comparerValue }.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_values_are_equal()
    {
        // Arrange
        var inputValue = 1024;
        var comparerValue = new ValueExpression { Value = 1024 };

        // Act
        var result = new LessThanOrEqualCondition { ComparerValue = comparerValue }.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_input_value_is_string_greater_than_comparer()
    {
        // Arrange
        var inputValue = "Zebra";
        var comparerValue = new ValueExpression { Value = "Fish" };

        // Act
        var result = new LessThanOrEqualCondition { ComparerValue = comparerValue }.Evaluate(Context, inputValue);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void test_evaluate_when_input_value_is_string_less_than_comparer()
    {
        // Arrange
        var inputValue = "Fish";
        var comparerValue = new ValueExpression { Value = "Zebra" };

        // Act
        var result = new LessThanOrEqualCondition { ComparerValue = comparerValue }.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_values_are_strings_and_equal()
    {
        // Arrange
        var inputValue = "Zebra Fish";
        var comparerValue = new ValueExpression { Value = "Zebra Fish" };

        // Act
        var result = new LessThanOrEqualCondition { ComparerValue = comparerValue }.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_input_value_is_not_a_number_or_string()
    {
        // Arrange
        var inputValue = new List<JSONValue>();
        var comparerValue = new ValueExpression { Value = new List<JSONValue>() };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new LessThanOrEqualCondition { ComparerValue = comparerValue }.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_comparer_type_does_not_match_input_value_type()
    {
        // Arrange
        var inputValue = "hello";
        var comparerValue = new ValueExpression { Value = 1024 };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new LessThanOrEqualCondition { ComparerValue = comparerValue }.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_comparer_type_does_not_match_number_input_value_type()
    {
        // Arrange
        var inputValue = 1024;
        var comparerValue = new ValueExpression { Value = "hello" };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new LessThanOrEqualCondition { ComparerValue = comparerValue }.Evaluate(Context, inputValue));
    }
}
