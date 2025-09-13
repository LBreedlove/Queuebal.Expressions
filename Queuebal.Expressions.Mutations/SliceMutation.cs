using System.Text;
using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class SliceMutation : Mutation
{
    public static string MutationType => "Slice";

    /// <summary>
    /// The position in the string to start collecting values from, inclusive.
    /// Defaults to 0.
    /// </summary>
    public IExpression Start { get; set; } = new ValueExpression { Value = 0 };

    /// <summary>
    /// The maximum index to collect, exclusive.
    /// If null, the entire list or string will be collected.
    /// </summary>
    public IExpression? Stop { get; set; } = null;

    /// <summary>
    /// The number of entries to move forward after taking a value.
    /// Must be greater than or equal to 1.
    /// </summary>
    public IExpression Step { get; set; } = new ValueExpression { Value = 1 };

    /// <summary>
    /// Indicates if the mutation is expecting to receive a string,
    /// rather than a list. This is used when the inputValue is null,
    /// so the Slice mutation knows what type of value to return.
    /// </summary>
    public bool ExpectingString { get; set; } = false;

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        // we want users to be able to select something that doesn't exist and call slice on it
        // this should just return an empty list
        if (inputValue.IsNull)
        {
            return ExpectingString
                ? ""
                : new List<JSONValue>();
        }

        if (!inputValue.IsList && !inputValue.IsString)
        {
            throw new InvalidOperationException("SliceMutation can only be applied to a list or string input value.");
        }

        var startValue = Start.Evaluate(context, inputValue);
        if (!startValue.IsInteger)
        {
            throw new InvalidOperationException("SliceMutation Start must evaluate to an integer.");
        }
        
        var stopValue = Stop?.Evaluate(context, inputValue);
        if (stopValue != null && !stopValue.IsInteger)
        {
            throw new InvalidOperationException("SliceMutation Stop must evaluate to an integer.");
        }

        var stepValue = Step.Evaluate(context, inputValue);
        if (!stepValue.IsInteger)
        {
            throw new InvalidOperationException("SliceMutation Step must evaluate to an integer.");
        }
        if (stepValue.IntValue < 1)
        {
            throw new InvalidOperationException("SliceMutation Step cannot be less than one");
        }

        if (startValue.IntValue < 0)
        {
            throw new InvalidOperationException("SliceMutation Start cannot be less than 0");
        }

        return inputValue.IsList
            ? GetListSlice(inputValue.ListValue, (int)startValue.IntValue, (int?)stopValue?.IntValue, (int)stepValue.IntValue)
            : GetStringSlice(inputValue.StringValue, (int)startValue.IntValue, (int?)stopValue?.IntValue, (int)stepValue.IntValue);
    }

    private JSONValue GetListSlice(List<JSONValue> inputValue, int start, int? stop, int step)
    {
        var output = new List<JSONValue>();
        int endPosition = stop.HasValue
            ? Math.Min(inputValue.Count, stop.Value)
            : inputValue.Count;

        if (endPosition < 0)
        {
            // if endPosition is negative, treat it as relative to the end of the list
            endPosition = inputValue.Count + endPosition;
        }

        for (int position = start; position < endPosition; position += step)
        {
            output.Add(inputValue[position]);
        }
        return output;
    }

    private string GetStringSlice(string inputValue, int start, int? stop, int step)
    {
        StringBuilder stringBuilder = new();
        int endPosition = stop.HasValue
            ? Math.Min(inputValue.Length, stop.Value)
            : inputValue.Length;

        if (endPosition < 0)
        {
            // if endPosition is negative, treat it as relative to the end of the list
            endPosition = inputValue.Length + endPosition;
        }

        for (int position = start; position < endPosition; position += step)
        {
            stringBuilder.Append(inputValue[position]);
        }

        return stringBuilder.ToString();
    }
}
