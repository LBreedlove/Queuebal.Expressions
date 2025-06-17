using Queuebal.Expressions;
using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class ToDateTimeMutation : Mutation
{
    /// <summary>
    /// Represents the type of mutation this class implements.
    /// </summary>
    public static string MutationType => "ToDateTime";

    /// <summary>
    /// Indicates if the DateTime should be converted to UTC,
    /// from local time, if the datetime does not end with 'Z' or a UTC offset.
    /// </summary>
    public bool ConvertToUtc { get; set; } = true;

    /// <summary>
    /// Evaluates the mutation by converting the input value to a DateTime.
    /// </summary>
    /// <param name="context">The context the mutation is running in.</param>
    /// <param name="inputValue">The value to convert to DateTime.</param>
    /// <returns>The DateTime representation of the input value.</returns>
    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (inputValue.IsString)
        {
            var result = DateTime.Parse(inputValue.StringValue);
            if (inputValue.StringValue.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
            {
                // If the string ends with 'Z', it is in UTC format
                return result.ToUniversalTime();
            }

            if (inputValue.StringValue.EndsWith("+00:00") || inputValue.StringValue.EndsWith("-00:00"))
            {
                // If the string ends with '+00:00' or '-00:00', it is in UTC offset format
                return result.ToUniversalTime();
            }

            if (ConvertToUtc)
            {
                // If ConvertToUtc is true, convert the local time to UTC
                return new JSONValue(result.ToUniversalTime());
            }

            // If ConvertToUtc is false, return the DateTime in the local time zone
            return new JSONValue(DateTime.SpecifyKind(result, DateTimeKind.Local));
        }

        if (inputValue.IsNumber)
        {
            // If the input is a number, assume it's a Unix timestamp in seconds
            return new JSONValue(DateTimeOffset.FromUnixTimeSeconds(inputValue.IntValue).UtcDateTime);
        }

        throw new InvalidOperationException("Input value must be a string or number representing a date and time.");
    }
}
