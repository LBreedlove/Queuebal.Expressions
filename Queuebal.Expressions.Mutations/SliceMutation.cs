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
    public int Start { get; set; } = 0;

    /// <summary>
    /// The maximum index to collect, exclusive.
    /// If null, the entire list or string will be collected.
    /// </summary>
    public int? Stop { get; set; }

    /// <summary>
    /// The number of entries to move forward after taking a value.
    /// Must be greater than or equal to 1.
    /// </summary>
    public int Step { get; set; } = 1;

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

        if (Step < 1)
        {
            throw new InvalidOperationException("SliceMutation Step cannot be less than one");
        }

        if (Start < 0)
        {
            throw new InvalidOperationException("SliceMutation Start cannot be less than 0");
        }

        return inputValue.IsList
            ? GetListSlice(inputValue.ListValue)
            : GetStringSlice(inputValue.StringValue);
    }

    private JSONValue GetListSlice(List<JSONValue> inputValue)
    {
        var output = new List<JSONValue>();
        int endPosition = Stop.HasValue
            ? Math.Min(inputValue.Count, Stop.Value)
            : inputValue.Count;

        for (int position = Start; position < endPosition; position += Step)
        {
            output.Add(inputValue[position]);
        }
        return output;
    }

    private string GetStringSlice(string inputValue)
    {
        StringBuilder stringBuilder = new();
        int endPosition = Stop.HasValue
            ? Math.Min(inputValue.Length, Stop.Value)
            : inputValue.Length;

        for (int position = Start; position < endPosition; position += Step)
        {
            stringBuilder.Append(inputValue[position]);
        }

        return stringBuilder.ToString();
    }
}
