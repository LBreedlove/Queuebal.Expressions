using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions.Mutations;


[TestClass]
public class TestAddDaysMutation
{
    [TestMethod]
    public void test_evaluate_when_days_is_int()
    {
        var mutation = new AddDaysMutation
        {
            InputValue = new MutationExpression
            {
                // convert the input string to a DateTime, UTC
                Mutation = new ToDateTimeMutation()
            },
            Days = new ValueExpression { Value = new JSONValue(5) }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());

        var inputValue = new JSONValue("2023-10-01T12:00:00Z");
        var result = mutation.Evaluate(context, inputValue);

        var expected = DateTime.Parse("2023-10-06T12:00:00Z").ToUniversalTime();
        Assert.AreEqual(expected, result.DateTimeValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_not_a_datetime()
    {
        var mutation = new AddDaysMutation
        {
            Days = new ValueExpression { Value = new JSONValue(5) }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());

        var inputValue = new JSONValue("not a datetime");
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_hours_is_not_a_number()
    {
        var mutation = new AddDaysMutation
        {
            Days = new ValueExpression { Value = new JSONValue("not a number") }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.DataProvider());

        var inputValue = new JSONValue(DateTime.Parse("2023-10-01T17:00:00Z").ToUniversalTime());
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }
}