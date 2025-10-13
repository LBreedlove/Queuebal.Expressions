using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions.Mutations;

[TestClass]
public class TestStringFormatMutation
{
    [TestMethod]
    public void test_evaluate_when_input_is_not_string_throws()
    {
        var mutation = new StringFormatMutation
        {
            Values = new Dictionary<string, IExpression>
            {
                { "Hello", new ValueExpression { Value = "World" } }
            }
        };

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue(123); // Not a string
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_formats_with_values()
    {
        var mutation = new StringFormatMutation
        {
            Values = new Dictionary<string, IExpression>
            {
                { "Name", new ValueExpression { Value = "Alice" } },
                { "Age", new ValueExpression { Value = 30 } }
            }
        };

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue("My name is ${Name} and I am ${Age} years old.");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsString);
        Assert.AreEqual("My name is Alice and I am 30 years old.", result.StringValue);
    }
}
