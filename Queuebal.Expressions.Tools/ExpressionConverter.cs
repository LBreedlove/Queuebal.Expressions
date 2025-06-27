using Queuebal.Expressions;
using Queuebal.Json;

namespace Queuebal.Expressions.Tools;


public static class ExpressionConverter
{
    /// <summary>
    /// Creates an IExpression from the provided dictionary.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IExpression ToExpression(this Dictionary<string, object?> source)
    {
        var output = new Dictionary<string, IExpression>();
        foreach (var kvp in source)
        {
            var valueExpression = kvp.Value switch
            {
                string value => new ValueExpression { Value = value },
                int value => new ValueExpression { Value = value },
                long value => new ValueExpression { Value = value },
                bool value => new ValueExpression { Value = value },
                float value => new ValueExpression { Value = value },
                double value => new ValueExpression { Value = value },
                DateTime value => new ValueExpression { Value = value },
                JSONValue value => new ValueExpression { Value = value },

                Dictionary<string, object?> value => value.ToExpression(),
                List<object?> value => value.ToExpression(),
                IExpression value => value,

                null => new ValueExpression { Value = new() },
                _ => throw new InvalidOperationException("Cannot convert dictionary entry value to expression"),
            };

            output[kvp.Key] = valueExpression;
        }

        return new DictExpression { Value = output };
    }

    public static IExpression ToExpression(this List<object?> source)
    {
        var output = new List<IExpression>();
        foreach (var entry in source)
        {
            var valueExpression = entry switch
            {
                string value => new ValueExpression { Value = value },
                int value => new ValueExpression { Value = value },
                long value => new ValueExpression { Value = value },
                bool value => new ValueExpression { Value = value },
                float value => new ValueExpression { Value = value },
                double value => new ValueExpression { Value = value },
                DateTime value => new ValueExpression { Value = value },
                JSONValue value => new ValueExpression { Value = value },

                Dictionary<string, object?> value => value.ToExpression(),
                List<object?> value => value.ToExpression(),
                IExpression value => value,

                null => new ValueExpression { Value = new() },
                _ => throw new InvalidOperationException("Cannot convert list value to expression"),
            };

            output.Add(valueExpression);
        }

        return new ListExpression { Value = output };
    }
}