using System.Text.Json;

using Queuebal.Serialization;
using Queuebal.Json;

namespace Queuebal.UnitTests.Serialization;

[TestClass]
public class TestJSONValueConverter
{
    [TestMethod]
    public void test_write_with_list_and_dict_values()
    {
        var value = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", 123 },
            { "key3", 3.14 },
            { "key4", false },
            { "key5", true },
            { "key_with_null", new JSONValue() },
            {
                "key6", new List<JSONValue>
                {
                    "value2",
                    245,
                    42.7,
                    true,
                    false,
                    new Dictionary<string, JSONValue>
                    {
                        { "key1a", "value3" },
                        { "key2a", 456 },
                        { "key3a", 7.92 },
                        { "key4a", true },
                        { "key5a", false },
                        {
                            "key6a", new List<JSONValue>
                            {
                                "value4",
                                678,
                                7.42,
                                false,
                                true,
                            }
                        }
                    }
                }
            }
        });

        var options = new JsonSerializerOptions();
        options.Converters.Add(new JSONValueConverter());
        var json = JsonSerializer.Serialize(value, options);

        // TODO: Figure out how to use JsonSerializer.Deserialize to deserialize JSONValue.
        var deserialized = new JSONValue(JsonDocument.Parse(json).RootElement);
        Assert.AreEqual(value, deserialized);
    }
}
 