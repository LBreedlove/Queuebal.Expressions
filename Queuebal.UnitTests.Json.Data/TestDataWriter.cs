using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.UnitTests.Json.Data;

[TestClass]
public class TestDataWriter
{
    [TestMethod]
    public void test_write_when_writing_to_empty_path_returns_value()
    {
        // Act
        var result = new DataWriter().WriteValue("", new JSONValue("test value"));

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("test value", result.StringValue);
    }

    [TestMethod]
    public void test_write_when_writing_to_empty_path_and_object_exists_throws_exception()
    {
        // Arrange
        var valuePaths = new[]
        {
            new DataWriterValuePath { Path = "existingField", Value = new JSONValue("existing value") },
            new DataWriterValuePath { Path = "", Value = new JSONValue("test value") }, // this should fail
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => new DataWriter().WriteValues(valuePaths));
    }

    [TestMethod]
    public void test_write_when_writing_field_to_object()
    {
        // Arrange
        string path = "simple.path.value";

        // Act
        var result = new DataWriter().WriteValue(path, new JSONValue("test value"));

        // Assert
        Assert.IsNotNull(result);

        var expected = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "simple", new JSONValue(new Dictionary<string, JSONValue>
                {
                    { "path", new JSONValue(new Dictionary<string, JSONValue>
                        {
                            { "value", new JSONValue("test value") }
                        })
                    }
                })
            }
        });

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_write_when_writing_object_into_list()
    {
        // Arrange
        string path = "simple.items[].value";

        // Act
        var result = new DataWriter().WriteValue(path, new JSONValue("test value"));

        // Assert
        Assert.IsNotNull(result);

        var expected = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "simple", new JSONValue(new Dictionary<string, JSONValue>
                {
                    { "items", new JSONValue(new List<JSONValue>
                        {
                            new Dictionary<string, JSONValue>
                            {
                                { "value", new JSONValue("test value") }
                            }
                        })
                    }
                })
            }
        });

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_write_when_writing_raw_value_over_raw_value_overwrites()
    {
        var result = new DataWriter().WriteValues
        (
            [
                new DataWriterValuePath { Path = "", Value = new JSONValue(123) },
                new DataWriterValuePath { Path = "", Value = new JSONValue("another test value") }
            ]
        );

        Assert.IsTrue(result.IsString);
        Assert.AreEqual("another test value", result.StringValue);
    }

    [TestMethod]
    public void test_write_with_multiple_writes_into_same_object()
    {
        // Arrange
        var dataWriter = new DataWriter();

        // Act
        var result = dataWriter.WriteValues
        (
            [
                new DataWriterValuePath { Path = "simple.path.items[0].value", Value = new JSONValue("item value") },
                new DataWriterValuePath { Path = "simple.path.items[]", Value = new JSONValue("another item value - direct") },
                new DataWriterValuePath { Path = "simple.path.value", Value = new JSONValue("test value") },
                new DataWriterValuePath { Path = "simple.another_path.value", Value = new JSONValue("another value") },
            ]
        );

        // Assert
        var expected = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "simple", new JSONValue(new Dictionary<string, JSONValue>
                {
                    { "path", new JSONValue(new Dictionary<string, JSONValue>
                        {
                            { "value", new JSONValue("test value") },
                            { "items", new JSONValue(new List<JSONValue>
                                {
                                    new Dictionary<string, JSONValue>
                                    {
                                        { "value", new JSONValue("item value") },
                                    },
                                    new JSONValue("another item value - direct")
                                })
                            },
                        })
                    },
                    { "another_path", new JSONValue(new Dictionary<string, JSONValue>
                        {
                            { "value", new JSONValue("another value") }
                        })
                    }
                })
            }
        });
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_write_when_first_node_is_list_accessor()
    {
        // Arrange
        string path = "[1].value";

        // Act
        var result = new DataWriter().WriteValue(path, new JSONValue("test value"));

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsList);
        Assert.AreEqual(2, result.ListValue.Count);
        var expected = new JSONValue(new List<JSONValue>
        {
            new JSONValue(),
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "value", new JSONValue("test value") }
            })
        });
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_write_when_writing_into_list_of_lists()
    {
        // Arrange
        string path = "[][1].value";

        // Act
        var result = new DataWriter().WriteValue(path, new JSONValue("test value"));

        // Assert
        var expected = new JSONValue(new List<JSONValue>
        {
            new JSONValue(new List<JSONValue>
            {
                new JSONValue(),
                new JSONValue(new Dictionary<string, JSONValue>
                {
                    { "value", new JSONValue("test value") }
                })
            })
        });

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_write_when_writing_into_list_of_lists_with_index()
    {
        // Arrange
        string path = "[1][1].value";

        // Act
        var result = new DataWriter().WriteValue(path, new JSONValue("test value"));

        // Assert
        var expected = new JSONValue(new List<JSONValue>
        {
            new JSONValue(),
            new JSONValue(new List<JSONValue>
            {
                new JSONValue(),
                new JSONValue(new Dictionary<string, JSONValue>
                {
                    { "value", new JSONValue("test value") }
                })
            })
        });

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_write_when_writing_into_list_of_lists_with_leaf_value()
    {
        // Arrange
        string path = "[1][1]";

        // Act
        var result = new DataWriter().WriteValue(path, new JSONValue("test value"));

        // Assert
        var expected = new JSONValue(new List<JSONValue>
        {
            new JSONValue(),
            new JSONValue(new List<JSONValue>
            {
                new JSONValue(),
                new JSONValue("test value")
            })
        });

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_write_when_writing_into_list_object_as_list()
    {
        // Arrange
        string path = "simple.items[].value[]";

        // Act
        var result = new DataWriter().WriteValue(path, new JSONValue("test value"));

        // Assert
        Assert.IsNotNull(result);

        var expected = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "simple", new JSONValue(new Dictionary<string, JSONValue>
                {
                    { "items", new JSONValue(new List<JSONValue>
                        {
                            new Dictionary<string, JSONValue>
                            {
                                { "value", new JSONValue(new List<JSONValue>()
                                    {
                                        new JSONValue("test value")
                                    })
                                }
                            }
                        })
                    }
                })
            }
        });

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_write_when_writing_list_data_into_non_list_throws_exception()
    {
        // Arrange
        var valuePaths = new[]
        {
            new DataWriterValuePath { Path = "simple.items.value", Value = new JSONValue("test value") },
            new DataWriterValuePath { Path = "simple.items[].value", Value = new JSONValue("test value") }, // this should fail
        };

        var dataWriter = new DataWriter();

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => dataWriter.WriteValues(valuePaths));
    }

    [TestMethod]
    public void test_write_when_field_exists_where_expecting_object_throws_exception()
    {
        // Arrange
        var valuePaths = new[]
        {
            new DataWriterValuePath { Path = "simple.value", Value = new JSONValue("test value") },
            new DataWriterValuePath { Path = "simple.value.item", Value = new JSONValue("test value") }, // this should fail
        };

        var dataWriter = new DataWriter();

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => dataWriter.WriteValues(valuePaths));
    }
}