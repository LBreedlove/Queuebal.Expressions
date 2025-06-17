using Queuebal.Json.Data;
using Queuebal.Json;

namespace Queuebal.UnitTests.Json.Data;


[TestClass]
public class TestDataSelector
{
    [TestMethod]
    public void test_get_value_when_simple_path()
    {
        // Arrange
        var dataSelector = new DataSelector("simple.path.test");
        var inputJson = """
        {
            "simple": {
                "path": {
                    "test": "value"
                }
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual("value", results[0].Value.StringValue);
        Assert.AreEqual("simple.path.test", results[0].Path);
        Assert.IsFalse(results[0].IsSelectedAsList);
    }

    [TestMethod]
    public void test_get_value_when_path_with_array_index()
    {
        // Arrange
        var dataSelector = new DataSelector("simple.path.items[:].value");
        var inputJson = """
        {
            "simple": {
                "path": {
                    "items": [
                        { "value": "first" },
                        { "value": "second" }
                    ]
                }
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);

        Assert.AreEqual("first", results[0].Value.StringValue);
        Assert.AreEqual("second", results[1].Value.StringValue);

        Assert.AreEqual("simple.path.items[0].value", results[0].Path);
        Assert.AreEqual("simple.path.items[1].value", results[1].Path);

        Assert.IsTrue(results[0].IsSelectedAsList);
        Assert.IsTrue(results[1].IsSelectedAsList);
    }


    [TestMethod]
    public void test_get_value_when_path_with_lists_of_lists()
    {
        // Arrange
        var dataSelector = new DataSelector("simple.path.items[:][1:].value");
        var inputJson = """
        {
            "simple": {
                "path": {
                    "items": [
                        [
                            { "value": "first" },
                            { "value": "second" }
                        ],
                        [
                            { "value": "third" },
                            { "value": "fourth" }
                        ],
                        [
                            "fifth",
                            "sixth"
                        ],
                        [
                            "seventh",
                            "eighth",
                            "ninth",
                            { "value": "tenth" }
                        ]
                    ]
                }
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(3, results.Count);

        var first = results[0].Value.StringValue;
        var second = results[1].Value.StringValue;
        var third = results[2].Value.StringValue;

        Assert.AreEqual("second", first);
        Assert.AreEqual("fourth", second);
        Assert.AreEqual("tenth", third);

        Assert.AreEqual("simple.path.items[0][1].value", results[0].Path);
        Assert.AreEqual("simple.path.items[1][1].value", results[1].Path);
        Assert.AreEqual("simple.path.items[3][3].value", results[2].Path);

        Assert.IsTrue(results[0].IsSelectedAsList);
        Assert.IsTrue(results[1].IsSelectedAsList);
        Assert.IsTrue(results[2].IsSelectedAsList);
    }

    [TestMethod]
    public void test_get_value_when_path_ends_with_list()
    {
        // Arrange
        var dataSelector = new DataSelector("simple.path.items[:][1:]");
        var inputJson = """
        {
            "simple": {
                "path": {
                    "items": [
                        [
                            { "value": "first" },
                            { "value": "second" }
                        ],
                        [
                            { "value": "third" },
                            { "value": "fourth" }
                        ],
                        [
                            "fifth",
                            "sixth"
                        ],
                        [
                            "seventh",
                            "eighth",
                            "ninth",
                            { "value": "tenth" }
                        ]
                    ]
                }
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(6, results.Count);

        var first = results[0].Value;
        var second = results[1].Value;
        var third = results[2].Value;
        var fourth = results[3].Value;
        var fifth = results[4].Value;
        var sixth = results[5].Value;

        Assert.AreEqual(new JSONValue(new Dictionary<string, JSONValue> { { "value", "second" } }), first);
        Assert.AreEqual(new JSONValue(new Dictionary<string, JSONValue> { { "value", "fourth" } }), second);
        Assert.AreEqual("sixth", third.StringValue);
        Assert.AreEqual("eighth", fourth.StringValue);
        Assert.AreEqual("ninth", fifth.StringValue);
        Assert.AreEqual(new JSONValue(new Dictionary<string, JSONValue> { { "value", "tenth" } }), sixth);

        Assert.AreEqual("simple.path.items[0][1]", results[0].Path);
        Assert.AreEqual("simple.path.items[1][1]", results[1].Path);
        Assert.AreEqual("simple.path.items[2][1]", results[2].Path);
        Assert.AreEqual("simple.path.items[3][1]", results[3].Path);
        Assert.AreEqual("simple.path.items[3][2]", results[4].Path);
        Assert.AreEqual("simple.path.items[3][3]", results[5].Path);
    }

    [TestMethod]
    public void test_get_value_when_path_ends_with_object()
    {
        // Arrange
        var dataSelector = new DataSelector("simple.path.items");
        var inputJson = """
        {
            "simple": {
                "path": {
                    "items": [
                        { "value": "first" },
                        { "value": "second" }
                    ]
                }
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        var resultDict1 = results[0].Value.ListValue[0].DictValue;
        var resultDict2 = results[0].Value.ListValue[1].DictValue;

        Assert.AreEqual(1, resultDict1.Keys.Count);
        Assert.AreEqual(1, resultDict2.Keys.Count);

        Assert.IsTrue(resultDict1["value"].StringValue == "first");
        Assert.IsTrue(resultDict2["value"].StringValue == "second");

        // because we didn't use an index segment at the end of the list,
        // we just selected the full list of items as a single value, so
        // there's only one result with the path being the list itself
        Assert.AreEqual("simple.path.items", results[0].Path);
    }

    [TestMethod]
    public void test_get_value_when_path_starts_with_list_segment()
    {
        // Arrange
        var dataSelector = new DataSelector("[:].items.value");
        var inputJson = """
        [
            { "items": { "value": "first" } },
            { "items": { "value": "second" } }
        ]
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);

        Assert.AreEqual("first", results[0].Value.StringValue);
        Assert.AreEqual("second", results[1].Value.StringValue);

        Assert.AreEqual("[0].items.value", results[0].Path);
        Assert.AreEqual("[1].items.value", results[1].Path);

        Assert.IsTrue(results[0].IsSelectedAsList);
        Assert.IsTrue(results[1].IsSelectedAsList);
    }

    [TestMethod]
    public void test_get_value_when_input_is_simple_value()
    {
        // Arrange
        var dataSelector = new DataSelector("");
        var inputValue = new JSONValue("test");

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("test", results[0].Value.StringValue);
        Assert.AreEqual("", results[0].Path);
        Assert.IsFalse(results[0].IsSelectedAsList);
    }

    [TestMethod]
    public void test_get_value_when_input_is_null()
    {
        // Arrange
        var dataSelector = new DataSelector("test.path");
        var inputValue = new JSONValue();

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(1, results.Count);
        Assert.IsTrue(results[0].Found == false);
        Assert.AreEqual("", results[0].Path);
    }

    [TestMethod]
    public void test_get_values_when_path_refers_to_list_that_is_not_list()
    {
        // Arrange
        var dataSelector = new DataSelector("test.path.items[:].value");
        var inputJson = """
        {
            "test": {
                "path": {
                    "items": {
                        "value": "not a list"
                    }
                }
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(1, results.Count);
        Assert.IsFalse(results[0].Found);
        Assert.AreEqual("test.path.items[:]", results[0].Path);
    }

    [TestMethod]
    public void test_get_values_from_path_where_input_is_not_an_object()
    {
        // Arrange
        var dataSelector = new DataSelector("test.path.items[:].value");
        var inputValue = new JSONValue("value");

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(1, results.Count);
        Assert.IsFalse(results[0].Found);
        Assert.AreEqual("test", results[0].Path);
    }

    [TestMethod]
    public void test_get_values_when_segment_in_object_not_found()
    {
        // Arrange
        var dataSelector = new DataSelector("test.invalid.items[:].value");
        var inputJson = """
        {
            "test": {
                "path": {
                    "items": [
                        { "value": "first" },
                        { "value": "second" }
                    ]
                }
            }
        }
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(1, results.Count);
        Assert.IsFalse(results[0].Found);
        Assert.AreEqual(results[0].Path, "test.invalid");
    }

    [TestMethod]
    public void test_get_values_from_list_with_more_items_than_index_range()
    {
        // Arrange
        var dataSelector = new DataSelector("[:2].value");
        var inputJson = """
        [
            { "value": "first"  },
            { "value": "second" },
            { "value": "third"  },
            { "value": "fourth" }
        ]
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);

        Assert.AreEqual(results[0].Value.StringValue, "first");
        Assert.AreEqual(results[1].Value.StringValue, "second");

        Assert.IsTrue(results[0].IsSelectedAsList);
        Assert.IsTrue(results[1].IsSelectedAsList);

        Assert.AreEqual(results[0].Path, "[0].value");
        Assert.AreEqual(results[1].Path, "[1].value");
    }

    [TestMethod]
    public void test_get_values_from_direct_index_when_selecting_from_list()
    {
        // Arrange
        var dataSelector = new DataSelector("[:].value[1]");
        var inputJson = """
        [
            { "value": ["first", "second"] },
            { "value": ["third", "fourth"] }
        ]
        """;

        var inputValue = new JSONValue(System.Text.Json.JsonDocument.Parse(inputJson).RootElement);

        // Act
        var results = dataSelector.GetValues(inputValue).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);

        Assert.AreEqual(results[0].Value.StringValue, "second");
        Assert.AreEqual(results[1].Value.StringValue, "fourth");

        Assert.IsTrue(results[0].IsSelectedAsList);
        Assert.IsTrue(results[1].IsSelectedAsList);

        Assert.AreEqual(results[0].Path, "[0].value[1]");
        Assert.AreEqual(results[1].Path, "[1].value[1]");
    }
}
