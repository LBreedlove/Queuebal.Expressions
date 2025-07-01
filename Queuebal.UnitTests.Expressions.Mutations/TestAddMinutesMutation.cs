using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions.Mutations;


[TestClass]
public class TestAddMinutesMutation
{
    [TestMethod]
    public void test_evaluate_when_hours_is_int()
    {
        var mutation = new AddMinutesMutation
        {
            InputValue = new MutationExpression
            {
                // convert the input string to a DateTime, UTC
                Mutation = new ToDateTimeMutation()
            },
            Minutes = new ValueExpression { Value = new JSONValue(5) }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());

        var inputValue = new JSONValue("2023-10-01T12:00:00Z");
        var result = mutation.Evaluate(context, inputValue);

        // expected has the 5 minutes added
        var expected = DateTime.Parse("2023-10-01T12:05:00Z").ToUniversalTime();
        Assert.AreEqual(expected, result.DateTimeValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_not_a_datetime()
    {
        var mutation = new AddMinutesMutation
        {
            Minutes = new ValueExpression { Value = new JSONValue(5) }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());

        var inputValue = new JSONValue("not a datetime");
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_hours_is_not_a_number()
    {
        var mutation = new AddMinutesMutation
        {
            Minutes = new ValueExpression { Value = new JSONValue("not a number") }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());

        var inputValue = new JSONValue(DateTime.Parse("2023-10-01T17:00:00Z").ToUniversalTime());
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }}