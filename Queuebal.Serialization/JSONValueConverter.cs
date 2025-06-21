using System.Text.Json;
using System.Text.Json.Serialization;

using Queuebal.Json;

namespace Queuebal.Serialization;

public class JSONValueConverter : JsonConverter<JSONValue>
{
    public override JSONValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new JSONValue(JsonDocument.ParseValue(ref reader).RootElement);
    }

    public override void Write(Utf8JsonWriter writer, JSONValue value, JsonSerializerOptions options)
    {
        if (value.IsNull)
        {
            writer.WriteNullValue();
            return;
        }

        switch (value.FieldType)
        {
            case JSONFieldType.String:
                writer.WriteStringValue(value.StringValue);
                break;
            case JSONFieldType.Float:
                writer.WriteNumberValue(value.FloatValue);
                break;
            case JSONFieldType.Integer:
                writer.WriteNumberValue(value.IntValue);
                break;
            case JSONFieldType.Boolean:
                writer.WriteBooleanValue(value.BooleanValue);
                break;
            case JSONFieldType.List:
                writer.WriteStartArray();
                foreach (var item in value.ListValue)
                {
                    JsonSerializer.Serialize(writer, item, options);
                }
                writer.WriteEndArray();
                break;
            case JSONFieldType.Dictionary:
                writer.WriteStartObject();
                foreach (var property in value.DictValue)
                {
                    writer.WritePropertyName(property.Key);
                    JsonSerializer.Serialize(writer, property.Value, options);
                }
                writer.WriteEndObject();
                break;
            default:
                throw new JsonException($"Unsupported JSON value type: {value.FieldType}");
        }
    }
}