using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions.Mutations;

[TestClass]
public class TestStringJoinMutation
{
    public TestStringJoinMutation()
    {
        Context = new ExpressionContext(new Json.Data.VariableProvider());
    }

    /// <summary>
    /// The ExpressionContext used in the tests.
    /// </summary>
    public ExpressionContext Context { get; }

    [TestMethod]
    public void test_evaluate_when_input_is_not_list_throws()
    {
        // Arrange
        var mutation = new StringJoinMutation { Separator = "." };
        var inputValue = "not a list";

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => mutation.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_list_contains_non_string_value_throws()
    {
        // Arrange
        var mutation = new StringJoinMutation { Separator = "." };
        var inputValue = new List<JSONValue>
        {
            "hello", "world", 123, false, "value"
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => mutation.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_combines_strings_with_separator()
    {
        // Arrange
        var mutation = new StringJoinMutation { Separator = "." };
        var inputValue = new List<JSONValue>
        {
            "hello", "world", "value"
        };

        // Act
        var result = mutation.Evaluate(Context, inputValue);

        // Assert
        Assert.AreEqual("hello.world.value", result.StringValue);
    }
}
