using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions.Mutations;


[TestClass]
public class TestToDateTimeMutation
{
    [TestMethod]
    public void test_evaluate_when_input_is_string_and_ends_with_z()
    {
        var mutation = new ToDateTimeMutation();

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue("2023-10-01T12:00:00Z");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsDateTime);
        Assert.AreEqual(new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc), result.DateTimeValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_and_ends_with_00_offset()
    {
        var mutation = new ToDateTimeMutation();

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue("2023-10-01T12:00:00+00:00");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsDateTime);
        Assert.AreEqual(new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc), result.DateTimeValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_and_ends_with_non_zero_offset()
    {
        var mutation = new ToDateTimeMutation();

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue("2023-10-01T12:00:00-07:00");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsDateTime);

        // because the offset is -07:00, the UTC time will be 7 hours ahead
        // so 12:00:00 + 7 hours = 19:00:00 UTC
        Assert.AreEqual(new DateTime(2023, 10, 1, 19, 0, 0, DateTimeKind.Utc), result.DateTimeValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_and_ends_with_non_zero_offset_and_convert_to_utc_is_false()
    {
        var mutation = new ToDateTimeMutation { ConvertToUtc = false };

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue("2023-10-01T12:00:00-07:00");
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsDateTime);

        // DateTime.Parse is correctly interpreting the time as -07:00 offset,
        // but then it converts it to our local timezone (which is UTC-05:00 in this case),
        // so it adds 2 hours to the time, making it 14:00:00.
        // To ensure the test works regardless of local timezone, we need to adjust the expectedResult
        // using the difference between the original offset (-07:00) and the current local offset.
        double originalOffsetSeconds = -7 * 3600; // -07:00 in seconds
        double offsetSeconds = DateTimeOffset.Now.Offset.TotalSeconds; // our local time offset in seconds
        double diffOffsetSeconds = offsetSeconds - originalOffsetSeconds;

        var expectedResult = new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Unspecified).AddSeconds(diffOffsetSeconds);
        Assert.AreEqual(expectedResult, result.DateTimeValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_string_and_not_a_valid_date_time()
    {
        var mutation = new ToDateTimeMutation();

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue("not a date time");

        Assert.ThrowsException<FormatException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_is_float()
    {
        var mutation = new ToDateTimeMutation();

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue(1727808000.0); // This is the Unix timestamp for 2024-10-01T18:40:00Z
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsDateTime);
        Assert.AreEqual(new DateTime(2024, 10, 1, 18, 40, 0, DateTimeKind.Utc), result.DateTimeValue);
    }


    [TestMethod]
    public void test_evaluate_when_input_is_int()
    {
        var mutation = new ToDateTimeMutation();

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue(1727808000); // This is the Unix timestamp for 2024-10-01T18:40:00Z
        var result = mutation.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsDateTime);
        Assert.AreEqual(new DateTime(2024, 10, 1, 18, 40, 0, DateTimeKind.Utc), result.DateTimeValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_not_string_or_number_throws_exception()
    {
        var mutation = new ToDateTimeMutation();

        var context = new ExpressionContext(new Json.Data.VariableProvider());
        var inputValue = new JSONValue(true); // Boolean is not a valid date time input

        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }
}
