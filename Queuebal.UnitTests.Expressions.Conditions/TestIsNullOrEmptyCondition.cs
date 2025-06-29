using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions.Conditions;


[TestClass]
public class TestIsNullOrEmptyCondition
{
    private ExpressionContext Context { get; } = new ExpressionContext(new Json.Data.DataProvider());

    [TestMethod]
    public void test_evaluate_when_value_is_null()
    {
        var condition = new IsNullOrEmptyCondition();
        var inputValue = new JSONValue();

        bool result = condition.Evaluate(Context, inputValue);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_value_is_string_and_not_empty()
    {
        var condition = new IsNullOrEmptyCondition();
        var inputValue = "test";

        bool result = condition.Evaluate(Context, inputValue);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void test_evaluate_when_value_is_string_and_empty()
    {
        var condition = new IsNullOrEmptyCondition();
        var inputValue = "";

        bool result = condition.Evaluate(Context, inputValue);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_value_is_list_and_not_empty()
    {
        var condition = new IsNullOrEmptyCondition();
        var inputValue = new List<JSONValue> { "test" };

        bool result = condition.Evaluate(Context, inputValue);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void test_evaluate_when_value_is_list_and_is_empty()
    {
        var condition = new IsNullOrEmptyCondition();
        var inputValue = new List<JSONValue> { };

        bool result = condition.Evaluate(Context, inputValue);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_evaluate_when_value_is_dict_and_not_empty()
    {
        var condition = new IsNullOrEmptyCondition();
        var inputValue = new Dictionary<string, JSONValue>
        {
            { "key", "test" }
        };

        bool result = condition.Evaluate(Context, inputValue);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void test_evaluate_when_value_is_dict_and_is_empty()
    {
        var condition = new IsNullOrEmptyCondition();
        var inputValue = new Dictionary<string, JSONValue> { };

        bool result = condition.Evaluate(Context, inputValue);
        Assert.IsTrue(result);
    }
}
