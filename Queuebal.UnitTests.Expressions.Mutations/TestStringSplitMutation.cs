using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions.Mutations;

[TestClass]
public class TestStringSplitMutation
{
    [TestMethod]
    public void test_evaluate_when_input_is_not_string_throws()
    {
        // Arrange
        var mutation = new StringSplitMutation
        {
            Separators = new List<string> { ";" }
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, new JSONValue(123)));
    }

    [TestMethod]
    public void test_evaluate_when_remove_empty_entries_removes_empties()
    {
        // Arrange
        var mutation = new StringSplitMutation
        {
            Separators = new List<string> { ";" },
            RemoveEmptyEntries = true,
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act
        var result = mutation.Evaluate(context, new JSONValue("value1;;value2;value3"));

        // Assert
        Assert.IsTrue(result.IsList);
        Assert.AreEqual(3, result.ListValue.Count);
        Assert.AreEqual("value1", result.ListValue[0].StringValue);
        Assert.AreEqual("value2", result.ListValue[1].StringValue);
        Assert.AreEqual("value3", result.ListValue[2].StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_trim_entries_trims_whitespace()
    {
        // Arrange
        var mutation = new StringSplitMutation
        {
            Separators = new List<string> { ";" },
            TrimEntries = true,
            RemoveEmptyEntries = true,
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // Act
        var result = mutation.Evaluate(context, new JSONValue("  value1 ; value2 ; ; value3  "));

        // Assert
        Assert.IsTrue(result.IsList);
        Assert.AreEqual(3, result.ListValue.Count);
        Assert.AreEqual("value1", result.ListValue[0].StringValue);
        Assert.AreEqual("value2", result.ListValue[1].StringValue);
        Assert.AreEqual("value3", result.ListValue[2].StringValue);
    }
}