using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions.Mutations;

[TestClass]
public class TestSliceMutation
{
    [TestMethod]
    public void test_evaluate_when_input_is_not_string_or_list_throws()
    {
        // Arrange
        var inputValue = 123;
        var mutation = new SliceMutation { Start = 0 };
        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_value_is_null_and_expecting_string_returns_empty_string()
    {
        // Arrange
        var inputValue = new JSONValue();
        var mutation = new SliceMutation
        {
            ExpectingString = true,
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act
        var result = mutation.Evaluate(context, inputValue);

        // Assert
        Assert.IsTrue(result.IsString);
        Assert.AreEqual("", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_value_is_null_and_not_expecting_string_returns_empty_list()
    {
        // Arrange
        var inputValue = new JSONValue();
        var mutation = new SliceMutation
        {
            ExpectingString = false,
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act
        var result = mutation.Evaluate(context, inputValue);

        // Assert
        Assert.IsTrue(result.IsList);
        Assert.AreEqual(0, result.ListValue.Count);
    }

    [TestMethod]
    public void test_evaluate_when_step_is_less_than_1_throws()
    {
        // Arrange
        var inputValue = "hello world";
        var mutation = new SliceMutation
        {
            Step = -1,
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_start_is_less_than_0_throws()
    {
        // Arrange
        var inputValue = "hello world";
        var mutation = new SliceMutation
        {
            Start = -1,
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_is_list_and_stop_is_greater_than_list_count()
    {
        // Arrange
        var inputValue = new List<JSONValue>
        {
            123,
            "hello",
            "world",
            false,
        };

        var mutation = new SliceMutation
        {
            Stop = 10,
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act
        var result = mutation.Evaluate(context, inputValue);

        // Assert
        Assert.IsTrue(result.IsList);
        Assert.AreEqual(4, result.ListValue.Count);

        var expected = new JSONValue(new List<JSONValue>
        {
            123,
            "hello",
            "world",
            false,
        });

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_list_and_step_is_greater_than_one()
    {
        // Arrange
        var inputValue = new List<JSONValue>
        {
            123,
            "hello",
            "world",
            false,
        };

        var mutation = new SliceMutation
        {
            Start = 1,
            Step = 2,
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act
        var result = mutation.Evaluate(context, inputValue);

        // Assert
        Assert.IsTrue(result.IsList);
        Assert.AreEqual(2, result.ListValue.Count);

        var expected = new JSONValue(new List<JSONValue>
        {
            "hello",
            false,
        });

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_and_stop_is_greater_than_string_length()
    {
        // Arrange
        var inputValue = "hello world";

        var mutation = new SliceMutation
        {
            Stop = inputValue.Length + 5,
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act
        var result = mutation.Evaluate(context, inputValue);

        // Assert
        var expected = "hello world";
        Assert.IsTrue(result.IsString);
        Assert.AreEqual(expected.Length, result.StringValue.Length);
        Assert.AreEqual(expected, result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_and_step_is_greater_than_one()
    {
        // Arrange
        var inputValue = "hello world";
        var mutation = new SliceMutation
        {
            Start = 1,
            Step = 2,
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act
        var result = mutation.Evaluate(context, inputValue);

        // Assert
        var expected = "el ol";
        Assert.IsTrue(result.IsString);
        Assert.AreEqual(expected.Length, result.StringValue.Length);
        Assert.AreEqual(expected, result.StringValue);
    }
}