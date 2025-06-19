using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions.Mutations;


[TestClass]
public class TestToLowerMutation
{
    [TestMethod]
    public void test_evaluate_when_input_is_not_string_throws()
    {
        var mutation = new ToLowerMutation();

        var context = new ExpressionContext(new Json.Data.DataProvider());

        var inputValue = new JSONValue(123); // Not a string
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_returns_uppercase()
    {
        var mutation = new ToLowerMutation();

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // TODO: Get a better test string that requires invariant-culture-specific casing
        var inputValue = new JSONValue("TeST StRiNg");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsString);
        Assert.AreEqual("test string", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_returns_uppercase_with_current_culture()
    {
        var mutation = new ToLowerMutation { UseInvariantCulture = false };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // TODO: Get a better test string that requires culture-specific casing
        var inputValue = new JSONValue("TeST StRiNg");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsString);
        Assert.AreEqual("test string", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_output_is_variable_replaces_value()
    {
        var mutation = new ToLowerMutation();

        var context = new ExpressionContext(new Json.Data.DataProvider());
        context.DataProvider.AddValue("output", new JSONValue("New output value"));

        var inputValue = new JSONValue("{OUTPUT}");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsString);
        Assert.AreEqual("New output value", result.StringValue);
    }
}