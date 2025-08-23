using Queuebal.Json;

namespace Queuebal.Expressions;


public class JoinField
{
    /// <summary>
    /// The name of the source field to include in the joined object.
    /// </summary>
    /// <remarks>
    /// This field may be '*' to indicate that all fields from the source object should be included.
    /// If '*' is used, the Alias property will be ignored.
    /// </remarks>
    public required string FieldName { get; set; }

    /// <summary>
    /// The name to use when writing the field to the joined object.
    /// If not specified, the original field name will be used.
    /// </summary>
    public string? Alias { get; set; }
}


/// <summary>
/// An expression that joins objects from two lists and creates a single list of objects, with
/// values from both lists.
/// NOTE: The performance of this expression is not optimal, as it uses a nested loop to join the objects.
/// It is recommended to use this expression only for small lists.
/// </summary>
public class JoinExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "Join";

    /// <summary>
    /// An expression that evaluates to the left list of objects to join.
    /// </summary>
    public required IExpression LTable { get; set; }

    /// <summary>
    /// An expression that evaluates to the right list of objects to join.
    /// </summary>
    public required IExpression RTable { get; set; }

    /// <summary>
    /// The key selector expression used to extract the key from the left objects for joining.
    /// </summary>
    /// <remarks>
    /// If the key selector is a RawValue with a string value, it will be treated as a field name to extract from the left objects.
    /// Otherwise, the input to the key selector will be a dict with the shape `{ "key": fieldName, "value": fieldValue }`.
    /// </remarks>
    public required IExpression LeftKeySelector { get; set; }

    /// <summary>
    /// The key selector expression used to extract the key from the right objects for joining.
    /// </summary>
    /// <remarks>
    /// If the key selector is a RawValue with a string value, it will be treated as a field name to extract from the right objects.
    /// Otherwise, the input to the key selector will be a dict with the shape `{ "key": fieldName, "value": fieldValue }`.
    /// </remarks>
    public required IExpression RightKeySelector { get; set; }

    /// <summary>
    /// The fields to include from the left table in the resulting joined objects.
    /// </summary>
    public required List<JoinField> LTableFields { get; set; }

    /// <summary>
    /// The fields to include from the right table in the resulting joined objects.
    /// </summary>
    public required List<JoinField> RTableFields { get; set; }

    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var lvalues = LTable.Evaluate(context, inputValue);
        if (!lvalues.IsList)
        {
            throw new InvalidOperationException("Left table must evaluate to a list.");
        }

        var rvalues = RTable.Evaluate(context, inputValue);
        if (!rvalues.IsList)
        {
            throw new InvalidOperationException("Right table must evaluate to a list.");
        }

        List<JSONValue> results = new();
        foreach (var lrecord in lvalues.ListValue)
        {
            if (!lrecord.IsDict)
            {
                throw new InvalidOperationException("Left table records must be dictionaries.");
            }

            var lkey = LeftKeySelector.Evaluate(context, lrecord);
            if (lkey.IsList || lkey.IsDict)
            {
                throw new InvalidOperationException("Left key selector cannot be a list or dictionary.");
            }

            foreach (var rrecord in rvalues.ListValue)
            {
                if (!rrecord.IsDict)
                {
                    throw new InvalidOperationException("Right table records must be dictionaries.");
                }

                var rkey = RightKeySelector.Evaluate(context, rrecord);
                if (rkey.IsList || rkey.IsDict)
                {
                    throw new InvalidOperationException("Right key selector cannot be a list or dictionary.");
                }

                if (!lkey.Equals(rkey))
                {
                    continue;
                }

                results.Add(CreateRecord(lrecord, rrecord));
            }
        }

        return results;
    }

    private JSONValue CreateRecord(JSONValue lrecord, JSONValue rrecord)
    {
        var result = new Dictionary<string, JSONValue>();
        foreach (var kvp in GetFields(lrecord, LTableFields))
        {
            result[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GetFields(rrecord, RTableFields))
        {
            result[kvp.Key] = kvp.Value;
        }

        return result;
    }

    private IEnumerable<KeyValuePair<string, JSONValue>> GetFields(JSONValue record, List<JoinField> fields)
    {
        if (fields.Count == 1 && fields[0].FieldName == "*")
        {
            foreach (var kvp in record.DictValue)
            {
                yield return new KeyValuePair<string, JSONValue>(kvp.Key, kvp.Value);
            }

            yield break;
        }

        foreach (var field in fields)
        {
            var fieldName = field.Alias ?? field.FieldName;
            if (record.DictValue.TryGetValue(field.FieldName, out var value))
            {
                yield return new KeyValuePair<string, JSONValue>(fieldName, value);
            }
            else
            {
                yield return new KeyValuePair<string, JSONValue>(fieldName, new JSONValue());
            }
        }
    }
}
