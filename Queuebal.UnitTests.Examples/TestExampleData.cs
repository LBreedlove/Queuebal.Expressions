using System.IO;
using System.Reflection;
using System.Text.Json;

using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;
using Queuebal.Json.Data;
using Queuebal.Serialization;
using Queuebal.Services;

namespace Queuebal.UnitTests.Examples;

[TestClass]
public class TestExampleData
{
    [TestMethod]
    [DynamicData(
        nameof(GetTestData),
        DynamicDataSourceType.Method,
        DynamicDataDisplayName = nameof(GetTestDisplayName))
    ]
    public void test_expression_output_matches_expected(object expectedResult, object[] inputData)
    {
        // Arrange
        // inputData[0] is a string containing the name of the test case.
        // inputData[1] is a JSONValue containing the data to transform.
        // inputData[2] is a deserialized expression to apply to the data to transform.
        // inputData[3] is an optional DataProvider that can be used for variable substitution in the expression.
        // expectedResult is a JSONValue containing the expected result of the transformation.
        if (inputData.Length < 3)
        {
            Assert.Fail("Input data does not contain enough elements. Expected at least 2 elements.");
            return;
        }

        var sourceData = inputData[1] as JSONValue;
        if (sourceData is null)
        {
            Assert.Fail("Source data is null. Ensure the test data is set up correctly.");
            return;
        }

        var expression = inputData[2] as IExpression;
        if (expression is null)
        {
            Assert.Fail("Expression is null. Ensure the test data is set up correctly.");
            return;
        }

        // DataProvider is an optional input we can use for variable substitution if the expression requires it.
        var dataProvider = new DataProvider();
        if (inputData.Length > 3 && inputData[3] is DataProvider)
        {
            dataProvider = (DataProvider)inputData[3];
        }

        var expected = (JSONValue)expectedResult;

        // Act
        var expressionResult = expression.Evaluate(new ExpressionContext(dataProvider), sourceData);

        // Assert
        Assert.AreEqual(expected, expressionResult);
    }

    /// <summary>
    /// Gets the display name of the test case using the supplied inputData.
    /// </summary>
    /// <param name="methodInfo">The MethodInfo for the method under test.</param>
    /// <param name="inputData">The inputData supplied to the test method.</param>
    /// <returns>A string representing the name of the test case.</returns>
    public static string GetTestDisplayName(MethodInfo methodInfo, object[] inputData)
    {
        // Use the first element of the data array as the display name for the test case
        return $"{methodInfo.Name}::{inputData[0] ?? "Unnamed Test"}";
    }

    /// <summary>
    /// Gets the data to use to parameterize the test method.
    /// Each item in the collection should be an array where the first element is the expected result
    /// and the subsequent elements are the input data for the test.
    /// This method can be used to provide multiple test cases for the same test method.
    /// The test method will be executed once for each item in the collection.
    /// </summary>
    /// <returns>
    /// An enumerable of object arrays, where the first element in the object array is the expected result,
    /// the second element is the data to transform, and the third element is the expression to apply to
    /// the data to transform to generate the expected result.
    /// </returns>
    private static IEnumerable<object[]> GetTestData()
    {
        // result[0] is a JSONValue containing the expected result of the transformation.
        // result[1][0] is a string containing the name of the test case.
        // result[1][1] is a JSONValue containing the data to transform.
        // result[1][2] is a deserialized expression to apply to the data to transform.
        // result[1][3] is an optional DataProvider that can be used for variable substitution in the expression.

        // yield a first test case that should always pass, i.e. test the test method itself
        yield return new object[]
        {
            new JSONValue("Transformed Data with Variable"),
            new object[]
            {
                "Sample Test Case",
                new JSONValue("Original Data"),
                new ValueExpression { Value = new JSONValue("Transformed Data {var_name}") },
                new DataProvider().AddValue("var_name", "with Variable"),
            }
        };

        // this sucks. We have to reference a type from the Conditions assembly and the Mutations assembly
        // for them to be loaded.
        var inputValue = new JSONValue("test");
        var mutation = new StringSplitMutation { Separators = new List<string> { "." } };
        var condition = new EqualsCondition { ComparerValue = new ValueExpression { Value = "test" } };
        mutation.Evaluate(new ExpressionContext(new DataProvider()), inputValue);
        condition.Evaluate(new ExpressionContext(new DataProvider()), inputValue);

        // build the type registry used to deserialize the expressions
        var expressionTypeRegistry = TypeRegistryService<IExpression>.BuildFromCurrentAppDomain("ExpressionType");
        var conditionTypeRegistry = TypeRegistryService<ICondition>.BuildFromCurrentAppDomain("ConditionType");
        var mutationTypeRegistry = TypeRegistryService<IMutation>.BuildFromCurrentAppDomain("MutationType");
        var typeResolver = new CompositeTypeResolver()
            .AddTypeRegistry(expressionTypeRegistry)
            .AddTypeRegistry(conditionTypeRegistry)
            .AddTypeRegistry(mutationTypeRegistry);

        // Iterate through all the input files in the Examples directory
        foreach (var file in Directory.GetFiles("Examples", "*.input.json"))
        {
            // Get the test name from the file name
            string[] filenameParts = Path.GetFileNameWithoutExtension(file).Split('.');
            if (filenameParts.Length < 2)
            {
                continue; // Skip files that do not have a valid test name
            }

            string testName = filenameParts[0];

            var outputFile = Path.Combine("Examples", $"{testName}.output.json");
            if (!File.Exists(outputFile))
            {
                continue; // Skip files that do not have a corresponding output file
            }

            var expressionFile = Path.Combine("Examples", $"{testName}.expression.json");
            if (!File.Exists(expressionFile))
            {
                continue; // Skip files that do not have a corresponding expression file
            }

            var dataProviderFile = Path.Combine("Examples", $"{testName}.data_provider.json");

            var inputData = BuildJsonValue(file);
            var outputData = BuildJsonValue(outputFile);
            var expression = BuildExpression(expressionFile, typeResolver);
            var dataProvider = BuildDataProvider(dataProviderFile);

            yield return new object[]
            {
                outputData,
                new object[]
                {
                    testName,
                    inputData,
                    expression,
                    dataProvider,
                }
            };
        }
    }

    /// <summary>
    /// Builds a JSONValue from the specified file path.
    /// </summary>
    /// <param name="filePath">The path of the file containing JSON data to load into a JSONValue.</param>
    /// <returns>A JSONValue representing the JSON data in the file.</returns>
    private static JSONValue BuildJsonValue(string filePath)
    {
        // Read the output file and deserialize it into a JSONValue
        var jsonContent = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            return new JSONValue(); // Return null if the file is empty
        }

        return new JSONValue(JsonDocument.Parse(jsonContent).RootElement);
    }

    /// <summary>
    /// Builds an expression from the JSON data in the specified file.
    /// </summary>
    /// <param name="filePath">The path of the JSON file containing the serialized expression.</param>
    /// <param name="typeResolver">The type resolver used to resolve the polymorphic types in the JSON data.</param>
    /// <returns>An IExpression built from the deserialized JSON.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the JSON does not deserialize into a non-null IExpression.</exception>
    private static IExpression BuildExpression(string filePath, CompositeTypeResolver typeResolver)
    {
        // Read the expression file and deserialize it into an IExpression
        var fileContent = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = typeResolver,
        };

        options.Converters.Add(new JSONValueConverter());
        return JsonSerializer.Deserialize<IExpression>(
            fileContent,
            options: options
        ) ?? throw new InvalidOperationException($"Failed to deserialize expression from file: {filePath}");
    }

    /// <summary>
    /// Builds a DataProvider with a single scope from the JSON data in the specified file.
    /// </summary>
    /// <param name="filePath">The path of the file containing the DataProvider data to deserialize.</param>
    /// <returns>A new DataProvider containing the values from the serialized JSON data.</returns>
    private static DataProvider BuildDataProvider(string filePath)
    {
        // Read the data provider file and deserialize it into a DataProvider
        if (!File.Exists(filePath))
        {
            return new DataProvider(); // Return an empty DataProvider if the file does not exist
        }

        var fileContent = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(fileContent))
        {
            return new DataProvider(); // Return an empty DataProvider if the file is empty
        }

        var jsonData = JsonSerializer.Deserialize<Dictionary<string, object?>>(fileContent);
        if (jsonData is null)
        {
            return new DataProvider(); // Return an empty DataProvider if the JSON data is null
        }

        var dataProvider = new DataProvider();
        foreach (var kv in jsonData)
        {
            switch (kv.Value)
            {
                case string strValue:
                    dataProvider.AddValue(kv.Key, new JSONValue(strValue));
                    break;
                case int intValue:
                    dataProvider.AddValue(kv.Key, new JSONValue(intValue));
                    break;
                case bool boolValue:
                    dataProvider.AddValue(kv.Key, new JSONValue(boolValue));
                    break;
                case double doubleValue:
                    dataProvider.AddValue(kv.Key, new JSONValue(doubleValue));
                    break;
                case List<object?> listValue:
                    dataProvider.AddValue(kv.Key, (JSONValue)listValue);
                    break;
                case Dictionary<string, object?> dictValue:
                    dataProvider.AddValue(kv.Key, (JSONValue)dictValue);
                    break;
                case JsonElement jsonElement:
                    // If the value is a JsonElement, convert it to JSONValue
                    dataProvider.AddValue(kv.Key, new JSONValue(jsonElement));
                    break;
                case null:
                    dataProvider.AddValue(kv.Key, new JSONValue());
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported value type: {kv.Value.GetType()} for key: {kv.Key}");
            }
        }
        return dataProvider;
    }
}
