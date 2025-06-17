using Queuebal.Json;
using Queuebal.Json.Data;
using Queuebal.Expressions;

namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestDataSelectorExpression
{
    public TestDataSelectorExpression()
    {
        Context = new ExpressionContext(new DataProvider());
    }

    /// <summary>
    /// The ExpressionContext used by the tests.
    /// </summary>
    protected ExpressionContext Context { get; }

    [TestMethod]
    public void test_evaluate_when_data_selector_returns_0_results_returns_empty_list()
    {
        // Arrange
        var path = "non.existent.path";
        var expression = new DataSelectorExpression() { Path = path };
        var inputJson = """
        {
            "some": {
                "other": "data"
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result.IsNull);
    }

    [TestMethod]
    public void test_evaluate_when_data_selector_returns_1_results_returns_json_object()
    {
        // Arrange
        var path = "simple.path";
        var expression = new DataSelectorExpression() { Path = path };
        var inputJson = """
        {
            "simple": {
                "path": "value"
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result.IsString);
        Assert.AreEqual(result.StringValue, "value");
    }

    [TestMethod]
    public void test_evaluate_when_include_selected_path_is_true()
    {
        // Arrange
        var path = "simple.path";
        var expression = new DataSelectorExpression() { Path = path, IncludeSelectedPath = true };
        var inputJson = """
        {
            "simple": {
                "path": "value"
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result.IsObject);
        Assert.AreEqual("value", result.DictValue["value"].StringValue);
        Assert.AreEqual("simple.path", result.DictValue["path"].StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_path_is_empty_returns_input_value_as_is()
    {
        // Arrange
        var expression = new DataSelectorExpression() { Path = "" };
        var inputJson = """
        {
            "some": {
                "other": "data"
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result.IsObject);
        Assert.AreEqual(inputValue, result.DictValue);
    }

    [TestMethod]
    public void test_evaluate_when_no_results_returns_empty_list()
    {
        // Arrange
        var path = "non.existent.path";
        var expression = new DataSelectorExpression() { Path = path };
        var inputJson = """
        {
            "some": {
                "other": "data"
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result.IsNull);
    }

    [TestMethod]
    public void test_evaluate_when_modifier_is_provided_applies_modifier_to_results()
    {
        // Arrange
        var path = "simple.path";
        var expression = new DataSelectorExpression()
        {
            Path = path,
            Modifier = new DataSelectorExpression()
            {
                Path = "path",
                IncludeSelectedPath = false,
            }
        };
        var inputJson = """
        {
            "simple": {
                "path": "value"
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result.IsString);
        Assert.AreEqual("simple.path", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_expression_has_input_value()
    {
        // Arrange
        var path = "";  // select the input_value as-is
        var expression = new DataSelectorExpression()
        {
            Path = path,
            InputValue = new ValueExpression { Value = "test" }
        };

        // note: this json won't actually get used by the selector
        // since the evaluated InputValue is the value we'll end up using
        // and our input value simply returns a new value.
        var inputJson = """
        {
            "simple": {
                "path": "value"
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result.IsString);
        Assert.AreEqual("test", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_selecting_from_list_returns_list_of_selected_items()
    {
        // Arrange
        var path = "items[].name";  // assuming we want to select names from a list of items
        var expression = new DataSelectorExpression() { Path = path };
        var inputJson = """
        {
            "items": [
                { "name": "item1" },
                { "name": "item2" },
                { "name": "item3" }
            ]
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result.IsList);
        Assert.AreEqual(3, result.ListValue.Count);
        Assert.AreEqual("item1", result.ListValue[0].StringValue);
        Assert.AreEqual("item2", result.ListValue[1].StringValue);
        Assert.AreEqual("item3", result.ListValue[2].StringValue);
    }
}
