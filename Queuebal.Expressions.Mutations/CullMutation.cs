using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


/// <summary>
/// A mutation that limits the depth of the input dictionary or list.
/// </summary>
public class CullMutation : Mutation
{
    public static string MutationType => "Cull";

    /// <summary>
    /// The maximum depth of the output value.
    /// Must be greater than or equal to 1.
    /// </summary>
    public required IExpression MaxDepth { get; set; }

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        var maxDepthValue = MaxDepth.Evaluate(context, inputValue);
        if (!maxDepthValue.IsInteger)
        {
            throw new InvalidOperationException("CullDictMutation MaxDepth must evaluate to an integer.");
        }

        if (maxDepthValue.IntValue < 1)
        {
            throw new InvalidOperationException("CullDictMutation MaxDepth must be greater than or equal to 1.");
        }

        if (inputValue.IsNull)
        {
            return new JSONValue();
        }

        if (!inputValue.IsDict && !inputValue.IsList)
        {
            return inputValue;
        }

        if (inputValue.IsList)
        {
            return CullList(inputValue.ListValue, maxDepthValue.IntValue);
        }

        return CullDict(inputValue.DictValue, maxDepthValue.IntValue);
    }

    private List<JSONValue> CullList(List<JSONValue> list, long depth)
    {
        if (depth == 0)
        {
            return new List<JSONValue>();
        }

        List<JSONValue> result = new();
        foreach (var item in list)
        {
            result.Add(GetCulledValue(item, depth));
        }

        return result;
    }

    private Dictionary<string, JSONValue> CullDict(Dictionary<string, JSONValue> dict, long depth)
    {
        if (depth == 0)
        {
            return new Dictionary<string, JSONValue>();
        }

        var culledDict = new Dictionary<string, JSONValue>();
        foreach (var (key, value) in dict)
        {
            culledDict[key] = GetCulledValue(value, depth - 1);
        }

        return culledDict;
    }

    private JSONValue GetCulledValue(JSONValue value, long depth)
    {
        if (value.IsDict)
        {
            return CullDict(value.DictValue, depth);
        }

        if (value.IsList)
        {
            return CullList(value.ListValue, depth);
        }

        return value;
    }
}