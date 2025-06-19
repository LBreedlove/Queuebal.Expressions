using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.Expressions;

/// <summary>
/// Represents an expression that selects data from a data source.
/// </summary>
public class DataSelectorExpression : Expression
{
    /// <summary>
    /// Gets the name of the expression type.
    /// This property MUST be overridden in derived classes to specify the type of expression.
    /// </summary>
    public static string ExpressionType { get; } = "DataSelector";

    /// <summary>
    /// The path in the data source to select data from.
    /// </summary>
    public required string Path { get; set; }

    /// <summary>
    /// Indicates if the path the item was selected from should be included in the result.
    /// If true, the results will be a list of JSON objects, with each object containing
    /// the selected value in a 'value' key, and the path it was selected from in a 'path' key.
    /// If false, the results will be a list of the selected JSON values directly, without
    /// being wrapped in another dictionary.
    /// </summary>
    /// <remarks
    /// If a Modifier is provided, the modifider will always receeive the path and value, as
    /// well as the original input value, regardless of this setting. This is used to provide
    /// contextual data to the modifier.
    /// </remarks>
    public bool IncludeSelectedPath { get; set; } = false;

    /// <summary>
    /// An optional expression that gets applied to the selected values, before
    /// returning them to the caller.
    /// </summary>
    public IExpression? Modifier { get; set; }

    /// <summary>
    /// Evaluates the expression and returns a list of the results.
    /// </summary>
    protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue)
    {
        var dataSelector = new DataSelector(Path);
        var results = dataSelector.GetValues(inputValue);

        bool selectedAsList = false;

        List<JSONValue> values = new();
        foreach (var result in results)
        {
            if (!result.Found)
            {
                // If the result was not found, we can skip it
                continue;
            }

            selectedAsList = result.IsSelectedAsList || selectedAsList;
            if (Modifier != null)
            {
                // If a modifier is provided, apply it to each result
                var modifierValue = new Dictionary<string, JSONValue>
                {
                    { "originalValue", inputValue   },
                    { "path",          result.Path  },
                    { "value",         result.Value },
                };

                values.Add(modifierValue);
            }
            else if (IncludeSelectedPath)
            {
                // If IncludeSelectedPath is true, wrap the value in a dictionary
                values.Add(new Dictionary<string, JSONValue>
                {
                    { "value", result.Value },
                    { "path", result.Path },
                });
            }
            else
            {
                values.Add(result.Value);
            }
        }

        if (values.Count > 1)
        {
            selectedAsList = true;
        }

        if (selectedAsList)
        {
            if (Modifier != null && values.Count > 0)
            {
                // If a modifier is provided, apply it to each result
                return Modifier.Evaluate(context, values);
            }

            return values;
        }

        if (Modifier != null && values.Count > 0)
        {
            // If a modifier is provided, apply it to each result
            return Modifier.Evaluate(context, values[0]);
        }

        if (values.Count == 0)
        {
            // If no values were found, return an empty JSONValue
            return new JSONValue();
        }

        return values[0];
    }
}
