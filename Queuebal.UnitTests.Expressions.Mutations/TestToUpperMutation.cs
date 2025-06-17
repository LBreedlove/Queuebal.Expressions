using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions.Mutations;


[TestClass]
public class TestToUpperMutation
{
    [TestMethod]
    public void test_evaluate_when_input_is_not_string_throws()
    {
        var mutation = new ToUpperMutation();

        var context = new ExpressionContext(new Json.Data.DataProvider());

        var inputValue = new JSONValue(123); // Not a string
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_returns_uppercase()
    {
        var mutation = new ToUpperMutation();

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // TODO: Get a better test string that requires invariant-culture-specific casing
        var inputValue = new JSONValue("test string");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsString);
        Assert.AreEqual("TEST STRING", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_returns_uppercase_with_current_culture()
    {
        var mutation = new ToUpperMutation { UseInvariantCulture = false };

        var context = new ExpressionContext(new Json.Data.DataProvider());

        // TODO: Get a better test string that requires culture-specific casing
        var inputValue = new JSONValue("test string");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsString);
        Assert.AreEqual("TEST STRING", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_output_is_variable_replaces_value()
    {
        var mutation = new ToUpperMutation();

        var context = new ExpressionContext(new Json.Data.DataProvider());
        context.DataProvider.AddValue("OUTPUT", new JSONValue("New output value"));

        var inputValue = new JSONValue("{output}");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsString);
        Assert.AreEqual("New output value", result.StringValue);
    }
}