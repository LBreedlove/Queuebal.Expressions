using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions.Mutations;

[TestClass]
public class TestRemoveNullKeysMutation
{
    public TestRemoveNullKeysMutation()
    {
        Context = new ExpressionContext(new Json.Data.DataProvider());
    }

    public ExpressionContext Context { get; }

    [TestMethod]
    public void test_evaluate_when_input_value_is_not_dict()
    {
        // Arrange
        var inputValue = new List<JSONValue> { 123, "test" };
        var expression = new RemoveNullKeysMutation();

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_recursive_is_false()
    {
        // Arrange
        var inputValue = new Dictionary<string, JSONValue>
        {
            { "0", 123 },
            { "1", "hello" },
            { "2", new JSONValue() },
            { "3", "world" },
            { "4", new Dictionary<string, JSONValue>
            {
                { "5", "second" },
                { "6", new JSONValue() },
                { "7", "level" },
            }}
        };

        var expression = new RemoveNullKeysMutation();

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        var expected = new Dictionary<string, JSONValue>
        {
            { "0", 123 },
            { "1", "hello" },
            { "3", "world" },
            { "4", new JSONValue(new Dictionary<string, JSONValue>
            {
                { "5", "second" },
                { "6", new JSONValue() },
                { "7", "level" },
            })}
        };

        CollectionAssert.AreEqual(expected, result.DictValue);
    }

    [TestMethod]
    public void test_evaluate_when_recursive()
    {
        // Arrange
        var inputValue = new Dictionary<string, JSONValue>
        {
            { "0", 123 },
            { "1", "hello" },
            { "2", new JSONValue() },
            { "3", "world" },
            { "4", new Dictionary<string, JSONValue>
            {
                { "5", "second" },
                { "6", new JSONValue() },
                { "7", "level" },
            }}
        };

        var expression = new RemoveNullKeysMutation()
        {
            Recursive = true
        };

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        var expected = new Dictionary<string, JSONValue>
        {
            { "0", 123 },
            { "1", "hello" },
            { "3", "world" },
            { "4", new JSONValue(new Dictionary<string, JSONValue>
            {
                { "5", "second" },
                { "7", "level" },
            })}
        };

        CollectionAssert.AreEqual(expected, result.DictValue);
    }
}
