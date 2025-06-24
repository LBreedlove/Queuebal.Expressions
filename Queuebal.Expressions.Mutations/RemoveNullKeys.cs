using System.Text;
using Queuebal.Json;

namespace Queuebal.Expressions.Mutations;


public class RemoveNullKeysMutation : Mutation
{
    public static string MutationType => "RemoveNullKeys";

    /// <summary>
    /// Indicates if the mutation should remove null keys from any child dictionaries too.
    /// </summary>
    public bool Recursive { get; set; } = false;

    protected override JSONValue EvaluateMutation(ExpressionContext context, JSONValue inputValue)
    {
        if (!inputValue.IsDict)
        {
            throw new InvalidOperationException("RemoveNullKeys can obly be applied to a dict input value");
        }

        var output = new Dictionary<string, JSONValue>();
        foreach (var kvp in inputValue.DictValue)
        {
            if (kvp.Value.IsNull)
            {
                continue;
            }

            var value = kvp.Value;
            if (Recursive && kvp.Value.IsDict)
            {
                value = EvaluateMutation(context, value);
            }

            output[kvp.Key] = value;
        }

        return output;
    }
}