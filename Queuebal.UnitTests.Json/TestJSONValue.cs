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
            { "key7", new Dictionary<string, object?>
            {
                { "key8", "dict value" }
            }}
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
            { "key7", new(new Dictionary<string, JSONValue>
                {
                    { "key8", new JSONValue("dict value") }
                }
            )}
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
            42,
            new Dictionary<string, object?>
            {
                { "key1", "value2 - string value" }
            }
        };

        var destValue = (JSONValue)sourceValue;
        var expected = new JSONValue(
            new List<JSONValue>
            {
                new("value1 - string value"),
                new(true),
                new(3.14),
                new(42),
                new Dictionary<string, JSONValue>
                {
                    { "key1", new("value2 - string value") }
                }
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
                42,
            }
        );

        var destValue = (List<object?>)sourceValue;
        var expected = new List<object>
        {
            "value1 - string value",
            true,
            3.14,
            (long)42,
        };

        CollectionAssert.AreEquivalent(expected, destValue);
    }

    [TestMethod]
    public void test_explicit_cast_to_dictionary_when_source_not_dictionary_throws()
    {
        var source = new JSONValue(true);
        Assert.ThrowsExactly<InvalidCastException>(() => (Dictionary<string, object?>)source);
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
                },
                {
                    "key_dict", new Dictionary<string, JSONValue>
                    {
                        { "value3", "dict string value" },
                    }
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
            },
            {
                "key_dict", new Dictionary<string, object?>
                {
                    { "value3", "dict string value" },
                }
            },
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
    public void test_explicit_cast_to_jsonvalue_dict()
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
                },
                {
                    "key_dict", new Dictionary<string, JSONValue>
                    {
                        { "value3", "dict string value" },
                    }
                }
           }
       );

        var dest = (Dictionary<string, JSONValue>)source;

        // CollectionAssert.AreEquivalent doesn't work when the dict contains a list.
        Assert.AreEqual(source.DictValue["key_string"], dest["key_string"]);
        Assert.AreEqual(source.DictValue["key_bool"], dest["key_bool"]);
        Assert.AreEqual(source.DictValue["key_float"], dest["key_float"]);
        Assert.AreEqual(source.DictValue["key_int"], dest["key_int"]);
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
    public void test_explicit_cast_to_long_when_value_is_not_number()
    {
        var source = new JSONValue("test");
        Assert.Throws<InvalidCastException>(() => (long)source);
    }

    [TestMethod]
    public void test_explicit_cast_to_int_when_value_is_number()
    {
        var source = new JSONValue(42.5);
        var result = (int)source;
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void test_explicit_cast_to_long_when_value_is_number()
    {
        var source = new JSONValue(42.5);
        var result = (long)source;
        Assert.AreEqual(42, result);
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

    [TestMethod]
    public void test_explicit_cast_to_datetime_when_value_is_datetime()
    {
        var now = DateTime.Now;
        var source = new JSONValue(now);
        var result = (DateTime)source;
        Assert.AreEqual(now, result);
    }

    [TestMethod]
    public void test_explicit_cast_to_datetime_when_value_is_utc_string()
    {
        var source = new JSONValue("2023-10-01T12:00:00Z");
        var result = (DateTime)source;
        Assert.AreEqual(new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc), result);
    }

    [TestMethod]
    public void test_explicit_cast_to_datetime_when_value_is_gmt_string()
    {
        var source = new JSONValue("2023-10-01T12:00:00+00:00");
        var result = (DateTime)source;
        Assert.AreEqual(new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc), result);
    }

    [TestMethod]
    public void test_explicit_cast_to_datetime_when_value_is_local_string()
    {
        var now = DateTime.Now;
        var source = new JSONValue(now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
        var result = (DateTime)source;
        Assert.AreEqual(now, result);
    }

    [TestMethod]
    public void test_explicit_cast_to_datetime_when_value_is_int()
    {
        var source = new JSONValue(1696156800); // Unix timestamp for 2023-10-01T10:40:00Z
        var result = (DateTime)source;
        Assert.AreEqual(new DateTime(2023, 10, 1, 10, 40, 0, DateTimeKind.Utc), result);
    }

    [TestMethod]
    public void test_explicit_cast_to_datetime_when_value_is_invalid()
    {
        var source = new JSONValue(true);
        Assert.Throws<InvalidCastException>(() => (DateTime)source);
    }

    [TestMethod]
    public void test_explicit_cast_to_list_when_value_contains_list_of_lists()
    {
        var source = new JSONValue(new List<JSONValue>
        {
            new JSONValue("item1"),
            new JSONValue(new List<JSONValue> { new JSONValue("subitem1"), new JSONValue(42) }),
            new JSONValue(),
            new JSONValue(true)
        });

        var expected = new JSONValue(new List<JSONValue>
        {
            new JSONValue("item1"),
            new JSONValue(new List<JSONValue> { new JSONValue("subitem1"), new JSONValue(42) }),
            new JSONValue(),
            new JSONValue(true)
        });

        var result = (List<object?>)source;
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_explict_cast_to_list_when_value_is_not_list()
    {
        var source = new JSONValue(true);
        Assert.ThrowsExactly<InvalidCastException>(() => (List<object?>)source);
    }

    [TestMethod]
    public void test_clone_when_value_is_null()
    {
        var source = new JSONValue();
        var clone = source.Clone();
        Assert.IsTrue(clone.IsNull);
    }

    [TestMethod]
    public void test_clone_when_value_is_boolean()
    {
        var source = new JSONValue(true);
        var clone = source.Clone();
        Assert.IsTrue(clone.IsBoolean);
        Assert.AreEqual(true, clone.BooleanValue);
    }

    [TestMethod]
    public void test_clone_when_value_is_number()
    {
        var source = new JSONValue(3.14);
        var clone = source.Clone();
        Assert.IsTrue(clone.IsNumber);
        Assert.AreEqual(3.14, clone.FloatValue);
    }

    [TestMethod]
    public void test_clone_when_value_is_integer()
    {
        var source = new JSONValue(42);
        var clone = source.Clone();
        Assert.IsTrue(clone.IsInteger);
        Assert.AreEqual(42, clone.IntValue);
    }

    [TestMethod]
    public void test_clone_when_value_is_datetime()
    {
        var source = new JSONValue(DateTime.Now);
        var clone = source.Clone();
        Assert.IsTrue(clone.IsDateTime);
        Assert.AreEqual(source.DateTimeValue, clone.DateTimeValue);
    }

    [TestMethod]
    public void test_clone_when_value_is_string()
    {
        var source = new JSONValue("test string");
        var clone = source.Clone();
        Assert.IsTrue(clone.IsString);
        Assert.AreEqual("test string", clone.StringValue);
    }

    [TestMethod]
    public void test_clone_when_value_is_list()
    {
        var source = new JSONValue(new List<JSONValue>
        {
            new JSONValue("item1"),
            new JSONValue(42),
            new JSONValue(true)
        });
        var clone = source.Clone();
        Assert.IsTrue(clone.IsList);
        Assert.AreEqual(3, clone.ListValue.Count);
        Assert.AreEqual("item1", clone.ListValue[0].StringValue);
        Assert.AreEqual(42, clone.ListValue[1].IntValue);
        Assert.IsTrue(clone.ListValue[2].BooleanValue);
    }

    [TestMethod]
    public void test_clone_when_value_is_dictionary()
    {
        var source = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "key1", new JSONValue("value1") },
            { "key2", new JSONValue(123) },
            { "key3", new JSONValue(false) }
        });
        var clone = source.Clone();
        Assert.IsTrue(clone.IsDict);
        Assert.AreEqual(3, clone.DictValue.Count);
        Assert.AreEqual("value1", clone.DictValue["key1"].StringValue);
        Assert.AreEqual(123, clone.DictValue["key2"].IntValue);
        Assert.IsFalse(clone.DictValue["key3"].BooleanValue);
    }

    [TestMethod]
    public void test_get_hash_code_for_all_types()
    {
        var datetimeValue = new JSONValue(DateTime.UtcNow);
        var datetimeValue2 = new JSONValue(DateTime.Now);
        var boolValue = new JSONValue(true);
        var boolValue2 = new JSONValue(false);
        var stringValue = new JSONValue("test");
        var stringValue2 = new JSONValue("hello");
        var floatValue = new JSONValue(3.14);
        var floatValue2 = new JSONValue(37.14);
        var intValue = new JSONValue(42);
        var intValue2 = new JSONValue(37);
        var listValue = new JSONValue(new List<JSONValue>{ new("test list 1") });
        var listValue2 = new JSONValue(new List<JSONValue>{ new("test list 2") });
        var dictValue = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "key1", "value1" }
        });
        var dictValue2 = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "key2", "value2" }
        });

        var datetimeValueHashCode = datetimeValue.GetHashCode();
        var datetimeValue2HashCode = datetimeValue2.GetHashCode();
        var boolValueHashCode = boolValue.GetHashCode();
        var boolValue2HashCode = boolValue2.GetHashCode();
        var stringValueHashCode = stringValue.GetHashCode();
        var stringValue2HashCode = stringValue2.GetHashCode();
        var floatValueHashCode = floatValue.GetHashCode();
        var floatValue2HashCode = floatValue2.GetHashCode();
        var intValueHashCode = intValue.GetHashCode();
        var intValue2HashCode = intValue2.GetHashCode();
        var listValueHashCode = listValue.GetHashCode();
        var listValue2HashCode = listValue2.GetHashCode();
        var dictValueHashCode = dictValue.GetHashCode();
        var dictValueHashCode2 = dictValue2.GetHashCode();

        Assert.AreNotEqual(datetimeValueHashCode, datetimeValue2HashCode);
        Assert.AreNotEqual(boolValueHashCode, boolValue2HashCode);
        Assert.AreNotEqual(stringValueHashCode, stringValue2HashCode);
        Assert.AreNotEqual(floatValueHashCode, floatValue2HashCode);
        Assert.AreNotEqual(intValueHashCode, intValue2HashCode);
        Assert.AreNotEqual(listValueHashCode, listValue2HashCode);
        Assert.AreNotEqual(dictValueHashCode, dictValueHashCode2);
    }
}
