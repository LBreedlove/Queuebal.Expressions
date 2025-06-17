using Queuebal.Json;

namespace Queuebal.UnitTests.Json;


[TestClass]
public class TestJSONValue
{
    [TestMethod]
    public void test_null_json_value()
    {
        var value = new JSONValue();
        Assert.IsTrue(value.IsNull);
    }

    [TestMethod]
    public void test_implicit_cast_from_dictionary()
    {
        var source = new Dictionary<string, object?>()
        {
            { "key2", "key2 - string value"},
            { "key3", true},
            { "key4", 3.14},
            { "key5", 42},
            { "key6", new List<object?>()
            {
                "key6 - list value",
                41.3,
                24,
                false,
            }},
        };

        var expected = new JSONValue(new Dictionary<string, JSONValue>()
        {
            { "key2", new("key2 - string value")},
            { "key3", new(true)},
            { "key4", new(3.14)},
            { "key5", new(42)},
            { "key6", new(new List<JSONValue>()
            {
                new("key6 - list value"),
                new(41.3),
                new(24),
                new(false),
            })},
        });

        var destValue = (JSONValue)source;
        Assert.AreEqual(expected, destValue);
    }

    [TestMethod]
    public void test_implicit_cast_from_list()
    {
        var sourceValue = new List<object?>
        {
            "value1 - string value",
            true,
            3.14,
            42
        };

        var destValue = (JSONValue)sourceValue;
        var expected = new JSONValue(
            new List<JSONValue>
            {
                new("value1 - string value"),
                new(true),
                new(3.14),
                new(42)
            }
        );

        Assert.AreEqual(expected, destValue);
    }

    [TestMethod]
    public void test_explicit_cast_to_list()
    {
        var sourceValue = new JSONValue(
            new List<JSONValue>
            {
                "value1 - string value",
                true,
                3.14,
                42
            }
        );

        var destValue = (List<object?>)sourceValue;
        var expected = new List<object?>
        {
            "value1 - string value",
            true,
            3.14,
            (long)42
        };

        CollectionAssert.AreEquivalent(expected, destValue);
    }

    [TestMethod]
    public void test_explicit_cast_to_dictionary()
    {
        var source = new JSONValue(
           new Dictionary<string, JSONValue>
           {
                { "key_string", new("value1 - string value") },
                { "key_bool", new(true) },
                { "key_float", new(3.14) },
                { "key_int", new(42) },
                {
                    "key_list",
                    new
                    (
                        new List<JSONValue>
                        {
                            new("value2 - string value"),
                            new(false),
                            new(14.3),
                            new(37),
                        }
                    )
                }
           }
       );

        var expected = new Dictionary<string, object?>()
        {
            { "key_string", "value1 - string value"},
            { "key_bool", true },
            { "key_float", 3.14 },
            { "key_int", (long)42 },
            {
                "key_list",
                new List<object?>
                {
                    "value2 - string value",
                    false,
                    14.3,
                    (long)37,
                }
            }
        };

        var dest = (Dictionary<string, object?>)source;
        // CollectionAssert.AreEquivalent doesn't work when the dict contains a list.
        Assert.AreEqual(expected["key_string"], dest["key_string"]);
        Assert.AreEqual(expected["key_bool"], dest["key_bool"]);
        Assert.AreEqual(expected["key_float"], dest["key_float"]);
        Assert.AreEqual(expected["key_int"], dest["key_int"]);

#pragma warning disable CS8600
        CollectionAssert.AreEqual((List<object?>)expected["key_list"], (List<object?>)dest["key_list"]);
#pragma warning restore CS8600
    }

    [TestMethod]
    public void test_to_string_when_value_is_dictionary()
    {
        var source = new JSONValue(
            new Dictionary<string, JSONValue>
            {
                { "key_string", new("value1 - string value") },
                { "key_bool", new(true) },
                { "key_float", new(3.14) },
                { "key_int", new(42) },
                {
                    "key_list",
                    new
                    (
                        new List<JSONValue>
                        {
                            new("value2 - string value"),
                            new(false),
                            new(14.3),
                            new(37),
                        }
                    )
                }
            }
        );

        var output = source.ToString();
        var expected = "{\"key_string\": \"value1 - string value\", \"key_bool\": true, \"key_float\": 3.14, \"key_int\": 42, \"key_list\": [\"value2 - string value\", false, 14.3, 37]}";
        Assert.AreEqual(expected, output);
    }

    [TestMethod]
    public void test_explicit_cast_to_bool_when_value_is_not_bool()
    {
        var source = new JSONValue("test");
        Assert.Throws<InvalidCastException>(() => (bool)source);
    }

    [TestMethod]
    public void test_explicit_cast_to_int_when_value_is_not_number()
    {
        var source = new JSONValue("test");
        Assert.Throws<InvalidCastException>(() => (int)source);
    }

    [TestMethod]
    public void test_explicit_cast_to_double_when_value_is_not_number()
    {
        var source = new JSONValue("test");
        Assert.Throws<InvalidCastException>(() => (double)source);
    }

    [TestMethod]
    public void test_explicit_cast_to_string_when_value_is_not_string()
    {
        var source = new JSONValue(123);
        Assert.Throws<InvalidCastException>(() => (string)source);
    }

    [TestMethod]
    public void test_explicit_cast_to_dict_when_value_is_not_dict()
    {
        var source = new JSONValue("test");
        Assert.Throws<InvalidCastException>(() => (Dictionary<string, JSONValue>)source);
    }

    [TestMethod]
    public void test_explicit_cast_to_list_when_value_is_not_list()
    {
        var source = new JSONValue("test");
        Assert.Throws<InvalidCastException>(() => (List<JSONValue>)source);
    }
}
