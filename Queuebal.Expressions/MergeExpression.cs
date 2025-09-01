using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.Expressions;


/// <summary>
/// An expression that merges or adds to values together.
/// </summary>
public class MergeExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "Merge";

    /// <summary>
    /// The value the RValue will be merged/added into.
    /// </summary>
    public required IExpression LValue { get; set; }

    /// <summary>
    /// The value to merge/add into the LValue.
    /// </summary>
    public required IExpression RValue { get; set; }

    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var lvalue = LValue.Evaluate(context, inputValue);
        var rvalue = RValue.Evaluate(context, inputValue);

        if (lvalue.IsDict)
        {
            return MergeDicts(lvalue, rvalue);
        }

        if (lvalue.IsList)
        {
            return MergeLists(lvalue, rvalue);
        }

        if (lvalue.IsString)
        {
            return MergeStrings(lvalue, rvalue);
        }

        if (lvalue.IsNumber)
        {
            return MergeNumbers(lvalue, rvalue);
        }

        throw new InvalidOperationException("Merge can only merge dicts, lists, strings, and numbers.");
    }

    private JSONValue MergeDicts(JSONValue lvalue, JSONValue rvalue)
    {
        if (!rvalue.IsDict)
        {
            throw new InvalidOperationException("Can only merge dicts with other dicts.");
        }

        var mergedDict = new Dictionary<string, JSONValue>(lvalue.DictValue);
        foreach (var (key, value) in rvalue.DictValue)
        {
            mergedDict[key] = value;
        }

        return new JSONValue(mergedDict);
    }

    private JSONValue MergeLists(JSONValue lvalue, JSONValue rvalue)
    {
        if (!rvalue.IsList)
        {
            throw new InvalidOperationException("Can only merge lists with other lists.");
        }

        var mergedList = new List<JSONValue>(lvalue.ListValue);
        mergedList.AddRange(rvalue.ListValue);
        return new JSONValue(mergedList);
    }

    private JSONValue MergeStrings(JSONValue lvalue, JSONValue rvalue)
    {
        if (!rvalue.IsString)
        {
            throw new InvalidOperationException("Can only merge strings with other strings.");
        }

        return new JSONValue(lvalue.StringValue + rvalue.StringValue);
    }

    private JSONValue MergeNumbers(JSONValue lvalue, JSONValue rvalue)
    {
        if (!rvalue.IsNumber)
        {
            throw new InvalidOperationException("Can only merge numbers with other numbers.");
        }

        if (lvalue.IsInteger && rvalue.IsInteger)
        {
            return new JSONValue(lvalue.IntValue + rvalue.IntValue);
        }

        if (lvalue.IsInteger && rvalue.IsFloat)
        {
            return new JSONValue(lvalue.IntValue + rvalue.FloatValue);
        }

        if (lvalue.IsFloat && rvalue.IsInteger)
        {
            return new JSONValue(lvalue.FloatValue + rvalue.IntValue);
        }

        return new JSONValue(lvalue.FloatValue + rvalue.FloatValue);
    }
}