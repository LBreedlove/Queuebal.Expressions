using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;


namespace Queuebal.UnitTests.Expressions.Mutations;

[TestClass]
public class TestStringTrimMutation
{
    private ExpressionContext Context { get; } = new ExpressionContext(new Json.Data.DataProvider());

    [TestMethod]
    public void test_evaluate_when_input_value_is_not_string_throws()
    {
        var inputValue = 42;
        var mutation = new StringTrimMutation();

        Assert.ThrowsExactly<InvalidOperationException>(() => mutation.Evaluate(Context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_trim_begin()
    {
        var inputValue = " \t\n hello world \t\n ";
        var mutation = new StringTrimMutation
        {
            TrimStart = true,
            TrimEnd = false,
        };

        var result = mutation.Evaluate(Context, inputValue);
        Assert.AreEqual("hello world \t\n ", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_trim_end()
    {
        var inputValue = " \t\n hello world \t\n ";
        var mutation = new StringTrimMutation
        {
            TrimStart = false,
            TrimEnd = true,
        };

        var result = mutation.Evaluate(Context, inputValue);
        Assert.AreEqual(" \t\n hello world", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_trim_both()
    {
        var inputValue = " \t\n hello world \t\n ";
        var mutation = new StringTrimMutation
        {
            TrimStart = true,
            TrimEnd = true,
        };

        var result = mutation.Evaluate(Context, inputValue);
        Assert.AreEqual("hello world", result.StringValue);
    }


    [TestMethod]
    public void test_evaluate_when_trim_neither()
    {
        var inputValue = " \t\n hello world \t\n ";
        var mutation = new StringTrimMutation
        {
            TrimStart = false,
            TrimEnd = false,
        };

        var result = mutation.Evaluate(Context, inputValue);
        Assert.AreEqual(" \t\n hello world \t\n ", result.StringValue);
    }
}