using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Queuebal.Json;


/// <summary>
/// The types that can be stored in a JSONValue.
/// </summary>
public enum JSONFieldType
{
    Null,
    String,
    Boolean,
    Integer,
    Float,
    Dictionary,
    List,
    DateTime,
}

/// <summary>
/// Represents a JSONValue or object.
/// </summary>
public class JSONValue
{
    /// <summary>
    /// Stores the value of the JSONValue, when the value is a string.
    /// </summary>
    private readonly string? _stringValue;

    /// <summary>
    /// Stores the value of the JSONValue, when the value is a bool.
    /// </summary>
    private readonly bool? _boolValue;

    /// <summary>
    /// Stores the value of the JSONValue, when the value is an long.
    /// </summary>
    private readonly long? _intValue;

    /// <summary>
    /// Stores the value of the JSONValue, when the value is a float.
    /// </summary>
    private readonly double? _floatValue;

    /// <summary>
    /// Stores the value of the JSONValue, when the value is a DateTime.
    /// </summary>
    /// <remarks
    /// Although JSON doesn't support DateTime natively, we can store a
    /// DateTime value, then convert it to a string when serializing to JSON.
    /// When deserializing a JsonElement, strings will not be converted to DateTime automatically,
    /// </remarks>
    private readonly DateTime? _dateTimeValue;

    /// <summary>
    /// Stores the value of the JSONValue, when the value is a list of JSONValue objects.
    /// </summary>
    private readonly List<JSONValue>? _listValue;

    /// <summary>
    /// Stores the value of the JSONValue, when the value is a dictionary of string to JSONValue objects.
    /// </summary>
    private readonly Dictionary<string, JSONValue>? _dictValue;

    /// <summary>
    /// The type of JSONValue that is stored in this JSONValue instance.
    /// </summary>
    private readonly JSONFieldType _fieldType;

    /// <summary>
    /// Initializes a new instance of the JSONValue class, with a value of null.
    /// </summary>
    public JSONValue()
    {
        _fieldType = JSONFieldType.Null;
    }

    /// <summary>
    /// Creates a new JSONValue from a JsonElement.
    /// </summary>
    /// <param name="jsonElement">The JsonElement to create a JSONValue for.</param>
    public JSONValue(JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Null:
                _fieldType = JSONFieldType.Null;
                break;
            case JsonValueKind.String:
                _fieldType = JSONFieldType.String;
                _stringValue = jsonElement.GetString();
                break;
            case JsonValueKind.False:
                _fieldType = JSONFieldType.Boolean;
                _boolValue = false;
                break;
            case JsonValueKind.True:
                _fieldType = JSONFieldType.Boolean;
                _boolValue = true;
                break;
            case JsonValueKind.Undefined:
                _fieldType = JSONFieldType.Null;
                break;
            case JsonValueKind.Number:
                if (jsonElement.TryGetInt64(out long intValue))
                {
                    _fieldType = JSONFieldType.Integer;
                    _intValue = intValue;
                }
                else if (jsonElement.TryGetDouble(out double floatValue))
                {
                    _fieldType = JSONFieldType.Float;
                    _floatValue = floatValue;
                }
                break;
            case JsonValueKind.Array:
                _fieldType = JSONFieldType.List;
                var listValue = new List<JSONValue>();
                foreach (var value in jsonElement.EnumerateArray())
                {
                    listValue.Add(new(value));
                }
                _listValue = listValue;
                break;
            case JsonValueKind.Object:
                _fieldType = JSONFieldType.Dictionary;
                var dict = new Dictionary<string, JSONValue>();
                foreach (var property in jsonElement.EnumerateObject())
                {
                    dict.Add(property.Name, new(property.Value));
                }
                _dictValue = dict;
                break;
        }
    }

    /// <summary>
    /// Initializes a new instance of the JSONValue class, with a value type of string.
    /// </summary>
    public JSONValue(string value)
    {
        _stringValue = value;
        _fieldType = JSONFieldType.String;
    }

    /// <summary>
    /// Initializes a new instance of the JSONValue class, with a value type of bool.
    /// </summary>
    public JSONValue(bool value)
    {
        _boolValue = value;
        _fieldType = JSONFieldType.Boolean;
    }

    /// <summary>
    /// Initializes a new instance of the JSONValue class, with a value type of integer.
    /// </summary>
    public JSONValue(long value)
    {
        _intValue = value;
        _fieldType = JSONFieldType.Integer;
    }

    /// <summary>
    /// Initializes a new instance of the JSONValue class, with a value type of integer.
    /// </summary>
    public JSONValue(int value)
    {
        _intValue = value;
        _fieldType = JSONFieldType.Integer;
    }

    /// <summary>
    /// Initializes a new instance of the JSONValue class, with a value type of double.
    /// </summary>
    public JSONValue(double value)
    {
        _floatValue = value;
        _fieldType = JSONFieldType.Float;
    }

    /// <summary>
    /// Initializes a new instance of the JSONValue class, with a value type of DateTime.
    /// </summary>
    public JSONValue(DateTime value)
    {
        _dateTimeValue = value;
        _fieldType = JSONFieldType.DateTime;
    }

    /// <summary>
    /// Initializes a new instance of the JSONValue class, with a value type of list.
    /// </summary>
    public JSONValue(List<JSONValue> value)
    {
        _listValue = value;
        _fieldType = JSONFieldType.List;
    }

    /// <summary>
    /// Initializes a new instance of the JSONValue class, with a value type of dictionary.
    /// </summary>
    public JSONValue(Dictionary<string, JSONValue> value)
    {
        _dictValue = value;
        _fieldType = JSONFieldType.Dictionary;
    }

    /// <summary>
    /// Creates a deep copy of the JSONValue and returns it.
    /// </summary>
    /// <returns>A new JSONValue containing a deep copy of the source value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the current JSONValue has an unsupported FieldType.</exception>
    public JSONValue Clone() => _fieldType switch
    {
        JSONFieldType.Null       => new JSONValue(),
        JSONFieldType.String     => new JSONValue(_stringValue!),
        JSONFieldType.Boolean    => new JSONValue((bool)_boolValue!),
        JSONFieldType.DateTime   => new JSONValue((DateTime)_dateTimeValue!),
        JSONFieldType.Integer    => new JSONValue((long)_intValue!),
        JSONFieldType.Float      => new JSONValue((double)_floatValue!),
        JSONFieldType.List       => CloneList(this),
        JSONFieldType.Dictionary => CloneObject(this),
        _ => throw new InvalidOperationException($"Invalid JSONValue field type: {_fieldType}"),
    };

    /// <summary>
    /// Clones a list JSONValue, recursively.
    /// </summary>
    /// <param name="source">The source object to clone.</param>
    /// <returns>A deep copy of th source object.</returns>
    private static JSONValue CloneList(JSONValue source)
    {
        var result = new List<JSONValue>();
        foreach (var item in source.ListValue)
        {
            result.Add(item.Clone());
        }

        return result;
    }

    /// <summary>
    /// Clones an object JSONValue, recursively.
    /// </summary>
    /// <param name="source">The source object to clone.</param>
    /// <returns>A deep copy of th source object.</returns>
    private static JSONValue CloneObject(JSONValue source)
    {
        var result = new Dictionary<string, JSONValue>();
        foreach (var kv in source.DictValue)
        {
            result[kv.Key] = kv.Value.Clone();
        }

        return result;
    }

    // we can disable these warnings, because we use the FieldType to determine which field
    // was set, and we don't allow users to create a JSONValue with a nullable reference/value-type
    // except when FieldType is JSONFieldType.Null.
#pragma warning disable CS8600, CS8603, CS8629

    /// <summary>
    /// Explicitly converts a JSONValue, holding a string value, to a string.
    /// </summary>
    /// <param name="value">The JSONValue to convert.</param>
    public static explicit operator string(JSONValue value)
    {
        if (value.FieldType != JSONFieldType.String)
        {
            throw new InvalidCastException("Cannot cast non-string JSONValue to string");
        }
        return value.StringValue;
    }

    /// <summary>
    /// Explicitly converts a JSONValue, holding a boolean value, to a bool.
    /// </summary>
    /// <param name="value">The JSONValue to convert.</param>
    public static explicit operator bool(JSONValue value)
    {
        if (value.FieldType != JSONFieldType.Boolean)
        {
            throw new InvalidCastException("Cannot cast non-boolean JSONValue to bool");
        }
        return value.BooleanValue;
    }

    /// <summary>
    /// Explicitly converts a JSONValue, holding a numeric value, to an int.
    /// </summary>
    /// <param name="value">The JSONValue to convert.</param>
    public static explicit operator int(JSONValue value)
    {
        if (value.FieldType != JSONFieldType.Integer && value.FieldType != JSONFieldType.Float)
        {
            throw new InvalidCastException("Cannot cast non-numeric JSONValue to long");
        }
        return (int)value.IntValue;
    }

    /// <summary>
    /// Explicitly converts a JSONValue, holding a numeric value, to an long.
    /// </summary>
    /// <param name="value">The JSONValue to convert.</param>
    public static explicit operator long(JSONValue value)
    {
        if (value.FieldType != JSONFieldType.Integer && value.FieldType != JSONFieldType.Float)
        {
            throw new InvalidCastException("Cannot cast non-numeric JSONValue to long");
        }
        return value.IntValue;
    }

    /// <summary>
    /// Explicitly converts a JSONValue, holding a numeric value, to a double.
    /// </summary>
    /// <param name="value">The JSONValue to convert.</param>
    public static explicit operator double(JSONValue value)
    {
        if (value.FieldType != JSONFieldType.Integer && value.FieldType != JSONFieldType.Float)
        {
            throw new InvalidCastException("Cannot cast non-numeric JSONValue to double");
        }
        return value.FloatValue;
    }

    /// <summary>
    /// Explicitly converts a JSONValue, holding a DateTime, string, or a numeric value, to a DateTime.
    /// </summary>
    /// <param name="value">The JSONValue to convert.</param>
    public static explicit operator DateTime(JSONValue value)
    {
        if (value.FieldType == JSONFieldType.DateTime)
        {
            return value.DateTimeValue;
        }
        else if (value.FieldType == JSONFieldType.String)
        {
            return ConvertStringToDateTime(value.StringValue);
        }
        else if (value.FieldType == JSONFieldType.Integer || value.FieldType == JSONFieldType.Float)
        {
            // If the value is a number, assume it's a Unix timestamp in seconds
            return DateTimeOffset.FromUnixTimeSeconds(value.IntValue).UtcDateTime;
        }

        throw new InvalidCastException("Unable to cast JSONValue to a DateTime");
    }

    /// <summary>
    /// Converts a string value to a DateTime, taking into account the format of the string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A DateTime reflecting the value represented by the string.</returns>
    private static DateTime ConvertStringToDateTime(string value)
    {
        DateTime result;
        try
        {
            // Attempt to parse the string as a DateTime
            result = DateTime.Parse(value);
        }
        catch (FormatException)
        {
            // If parsing fails, throw an exception
            throw new InvalidCastException($"The string '{value}' is not in a valid DateTime format.");
        }

        if (value.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
        {
            // If the string ends with 'Z', it is in UTC format
            return result.ToUniversalTime();
        }

        if (value.EndsWith("+00:00") || value.EndsWith("-00:00"))
        {
            // If the string ends with '+00:00' or '-00:00', it is in UTC offset format
            return result.ToUniversalTime();
        }

        // If ConvertToUtc is false, return the DateTime in the local time zone
        return DateTime.SpecifyKind(result, DateTimeKind.Local);
    }

    /// <summary>
    /// Explicitly converts a List JSONValue to a List of objects.
    /// </summary>
    /// <param name="value">The JSONValue to convert.</param>
    public static explicit operator List<object?>(JSONValue value)
    {
        if (value.FieldType != JSONFieldType.List)
        {
            throw new InvalidCastException("Cannot cast non-list JSONValue to List<object?>");
        }

        var result = new List<object?>();

        var data = value.ListValue;
        foreach (var listJSONValue in data)
        {
            if (listJSONValue.FieldType == JSONFieldType.Null)
            {
                result.Add(null);
            }
            else if (listJSONValue.FieldType == JSONFieldType.String)
            {
                result.Add((string)listJSONValue);
            }
            else if (listJSONValue.FieldType == JSONFieldType.Boolean)
            {
                result.Add((bool)listJSONValue);
            }
            else if (listJSONValue.FieldType == JSONFieldType.Float)
            {
                result.Add((double)listJSONValue);
            }
            else if (listJSONValue.FieldType == JSONFieldType.Integer)
            {
                result.Add((long)listJSONValue);
            }
            else if (listJSONValue.FieldType == JSONFieldType.List)
            {
                result.Add((List<object?>)listJSONValue);
            }
            else if (listJSONValue.FieldType == JSONFieldType.Dictionary)
            {
                result.Add((Dictionary<string, object?>)listJSONValue);
            }
            else
            {
                throw new InvalidCastException("Unexpected data type stored in JSONValue list");
            }
        }

        return result;
    }

    /// <summary>
    /// Explicitly converts a JSONValue holding a dictionary to a Dictionary<string, object?> value.
    /// </summary>
    /// <remarks>
    /// This operator recursively casts the elements of the dictionary in the JSONValue. Depending on
    /// the size of your dictionary data, this call may take longer than expected.
    /// </remarks>
    /// <param name="value">The JSONValue to convert.</param>
    public static explicit operator Dictionary<string, object?>(JSONValue value)
    {
        if (value.FieldType != JSONFieldType.Dictionary)
        {
            throw new InvalidCastException("Cannot cast non-dictionary JSONValue to Dictionary<string, object?>");
        }

        Dictionary<string, object?> result = new();
        foreach (var keyValuePair in value.DictValue)
        {
            if (keyValuePair.Value.FieldType == JSONFieldType.String)
            {
                result.Add(keyValuePair.Key, (string)keyValuePair.Value);
            }
            else if (keyValuePair.Value.FieldType == JSONFieldType.Boolean)
            {
                result.Add(keyValuePair.Key, (bool)keyValuePair.Value);
            }
            else if (keyValuePair.Value.FieldType == JSONFieldType.Float)
            {
                result.Add(keyValuePair.Key, (double)keyValuePair.Value);
            }
            else if (keyValuePair.Value.FieldType == JSONFieldType.Integer)
            {
                result.Add(keyValuePair.Key, (long)keyValuePair.Value);
            }
            else if (keyValuePair.Value.FieldType == JSONFieldType.List)
            {
                result.Add(keyValuePair.Key, (List<object?>)keyValuePair.Value);
            }
            else if (keyValuePair.Value.FieldType == JSONFieldType.Dictionary)
            {
                result.Add(keyValuePair.Key, (Dictionary<string, object?>)keyValuePair.Value);
            }
            else
            {
                throw new InvalidCastException("Unexpected data type stored in JSONValue dictionary");
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a JSONValue to a Dictionary<string, JSONValue> object.
    /// </summary>
    /// <param name="value">The JSONValue to convert.</param>
    public static explicit operator Dictionary<string, JSONValue>(JSONValue value)
    {
        if (value.FieldType != JSONFieldType.Dictionary)
        {
            throw new InvalidCastException("Cannot cast non-dictionary JSONValue to Dictionary<string, JSONValue>");
        }

        Dictionary<string, JSONValue> output = new();
        if (value._dictValue == null)
        {
            return output;
        }

        foreach (var keyValue in value._dictValue)
        {
            output[keyValue.Key] = keyValue.Value;
        }

        return output;
    }

    /// <summary>
    /// Converts a JSONValue to a List<JSONValue> object.
    /// </summary>
    /// <param name="value">The JSONValue to convert.</param>
    public static explicit operator List<JSONValue>(JSONValue value)
    {
        if (value.FieldType != JSONFieldType.List)
        {
            throw new InvalidCastException("Cannot cast non-list JSONValue to List<JSONValue>");
        }

        return value.ListValue.Select(v => v).ToList();
    }

    /// <summary>
    /// Implicitly converts a string value to a JSONValue.
    /// </summary>
    /// <param name="value">The value to store in the JSONValue.</param>
    public static implicit operator JSONValue(string value) => new(value);

    /// <summary>
    /// Implicitly converts a boolean value to a JSONValue.
    /// </summary>
    /// <param name="value">The value to store in the JSONValue.</param>
    public static implicit operator JSONValue(bool value) => new(value);

    /// <summary>
    /// Implicitly converts an integer value to a JSONValue.
    /// </summary>
    /// <param name="value">The value to store in the JSONValue.</param>
    public static implicit operator JSONValue(int value) => new(value);

    /// <summary>
    /// Implicitly converts a long value to a JSONValue.
    /// </summary>
    /// <param name="value">The value to store in the JSONValue.</param>
    public static implicit operator JSONValue(long value) => new(value);

    /// <summary>
    /// Implicitly converts a double value to a JSONValue.
    /// </summary>
    /// <param name="value">The value to store in the JSONValue.</param>
    public static implicit operator JSONValue(double value) => new(value);

    /// <summary>
    /// Implicitly converts a DateTime value to a JSONValue.
    /// </summary>
    /// <param name="value">The value to store in the JSONValue.</param>
    public static implicit operator JSONValue(DateTime value) => new(value);

    /// <summary>
    /// Implicitly converts a double value to a JSONValue.
    /// </summary>
    /// <param name="value">The value to store in the JSONValue.</param>
    public static implicit operator JSONValue(float value) => new((double)value);

    /// <summary>
    /// Implicitly converts an array of object?'s into a JSONValue.
    /// </summary>
    /// <param name="value">The value to store in the JSONValue.</param>
    public static implicit operator JSONValue(object?[] value)
    {
        return value.ToList();
    }

    /// <summary>
    /// Implicitly converts a List of object?'s into a JSONValue.
    /// </summary>
    /// <param name="value">The value to store in the JSONValue.</param>
    public static implicit operator JSONValue(List<object?> value)
    {
        List<JSONValue> result = new();
        foreach (var listValue in value)
        {
            var valueType = listValue?.GetType();
            if (listValue == null)
            {
                result.Add(new());
            }
            else if (valueType == typeof(string))
            {
                result.Add((string)listValue);
            }
            else if (valueType == typeof(bool))
            {
                result.Add((bool)listValue);
            }
            else if (valueType == typeof(long))
            {
                result.Add((long)listValue);
            }
            else if (valueType == typeof(int))
            {
                result.Add((int)listValue);
            }
            else if (valueType == typeof(double))
            {
                result.Add((double)listValue);
            }
            else if (valueType == typeof(DateTime))
            {
                result.Add((DateTime)listValue);
            }
            else if (valueType == typeof(List<object?>))
            {
                result.Add((List<object?>)listValue);
            }
            else if (valueType == typeof(List<object>))
            {
                result.Add((List<object?>)listValue);
            }
            else if (valueType == typeof(Dictionary<string, object?>))
            {
                result.Add((Dictionary<string, object?>)listValue);
            }
            else if (valueType == typeof(Dictionary<string, object>))
            {
                result.Add((Dictionary<string, object?>)listValue);
            }
            else if (valueType == typeof(JsonElement))
            {
                result.Add(new((JsonElement)listValue));
            }
        }
        return new JSONValue(result);
    }

    /// <summary>
    /// Implicitly converts an Dictionary of object?'s into a JSONValue.
    /// </summary>
    /// <param name="value">The value to store in the JSONValue.</param>
    public static implicit operator JSONValue(Dictionary<string, object?> sourceValue)
    {
        Dictionary<string, JSONValue> result = new();
        foreach (var keyValue in sourceValue)
        {
            var key = keyValue.Key;
            var value = keyValue.Value;
            var valueType = value?.GetType();

            if (value == null)
            {
                result.Add(key, new());
            }
            else if (valueType == typeof(string))
            {
                result.Add(key, (string)value);
            }
            else if (valueType == typeof(bool))
            {
                result.Add(key, (bool)value);
            }
            else if (valueType == typeof(long))
            {
                result.Add(key, (long)value);
            }
            else if (valueType == typeof(int))
            {
                result.Add(key, (int)value);
            }
            else if (valueType == typeof(double))
            {
                result.Add(key, (double)value);
            }
            else if (valueType == typeof(DateTime))
            {
                result.Add(key, (DateTime)value);
            }
            else if (valueType == typeof(float))
            {
                result.Add(key, (double)(float)value);
            }
            else if (valueType == typeof(List<object?>))
            {
                result.Add(key, (List<object?>)value);
            }
            else if (valueType == typeof(Dictionary<string, object?>))
            {
                result.Add(key, (Dictionary<string, object?>)value);
            }
            else if (valueType == typeof(JsonElement))
            {
                result.Add(key, new((JsonElement)value));
            }
        }
        return new JSONValue(result);
    }

    /// <summary>
    /// Creates a JSONValue containing a list of JSONValue's.
    /// </summary>
    /// <param name="sourceValue">The source item to convert to a JSONValue.</param>
    public static implicit operator JSONValue(List<JSONValue> sourceValue) => new(sourceValue);

    /// <summary>
    /// Converts a Dictionary<string, JSONValue> to a JSONValue wrapping the dictionary.
    /// </summary>
    /// <param name="sourceValue">The source value to convert to a JSONValue.</param>
    public static implicit operator JSONValue(Dictionary<string, JSONValue> sourceValue) => new JSONValue(sourceValue);

    /// <summary>
    /// Gets the string value of the object, or raises an InvalidOperationException if a string value was not used
    /// to set the JSONValue.
    /// </summary>
    public string StringValue => FieldType == JSONFieldType.String ? _stringValue : throw new InvalidOperationException();

    /// <summary>
    /// Gets the boolean value of the object, or raises an InvalidOperationException if a bool value was not used
    /// to set the JSONValue.
    /// </summary>
    public bool BooleanValue => FieldType == JSONFieldType.Boolean ? (bool)_boolValue : throw new InvalidOperationException();

    /// <summary>
    /// Gets the DateTime value of the object, or raises an InvalidOperationException if a DateTime value was not used
    /// to set the JSONValue.
    /// </summary>
    public DateTime DateTimeValue => FieldType == JSONFieldType.DateTime ? (DateTime)_dateTimeValue : throw new InvalidOperationException();

    /// <summary>
    /// Gets the double value of the object, or raises an InvalidOperationException if a double value was not used
    /// to set the JSONValue.
    /// </summary>
    public double FloatValue
    {
        get
        {
            if (FieldType == JSONFieldType.Float)
            {
                return (double)_floatValue;
            }

            if (FieldType == JSONFieldType.Integer)
            {
                return (double)_intValue;
            }

            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Gets the integer value of the object, or raises an InvalidOperationException if an long value was not used
    /// to set the JSONValue.
    /// </summary>
    public long IntValue
    {
        get
        {
            if (FieldType == JSONFieldType.Integer)
            {
                return (long)_intValue;
            }

            if (FieldType == JSONFieldType.Float)
            {
                return (long)_floatValue;
            }

            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Gets the list value of the object, or raises an InvalidOperationException if a list value was not used
    /// to set the JSONValue.
    /// </summary>
    public List<JSONValue> ListValue => FieldType == JSONFieldType.List ? _listValue : throw new InvalidOperationException();

    /// <summary>
    /// Gets the Dictionary value of the object, or raises an InvalidOperationException if a dict value was not used
    /// to set the JSONValue.
    /// </summary>
    public Dictionary<string, JSONValue> DictValue => FieldType == JSONFieldType.Dictionary ? _dictValue : throw new InvalidOperationException();

    #pragma warning restore CS8600, CS8603, CS8629

    /// <summary>
    /// Indicates if the JSONValue stores a list value.
    /// </summary>
    public bool IsList => _fieldType == JSONFieldType.List;

    /// <summary>
    /// Indicates if the JSONValue stores an object/dictionary value.
    /// </summary>
    public bool IsObject => _fieldType == JSONFieldType.Dictionary;

    /// <summary>
    /// Indicates if the JSONValue stores a numeric value (float or integer).
    /// </summary>
    public bool IsNumber => _fieldType == JSONFieldType.Integer || _fieldType == JSONFieldType.Float;

    /// <summary>
    /// Indicates if the JSONValue stores a floating point value (float or double).
    /// </summary>
    public bool IsFloat => _fieldType == JSONFieldType.Float;

    /// <summary>
    /// Indicates if the JSONValue stores an integer value (long or int).
    /// </summary>
    public bool IsInteger => _fieldType == JSONFieldType.Integer;

    /// <summary>
    /// Indicates if the JSONValue stores a string value.
    /// </summary>
    public bool IsString => _fieldType == JSONFieldType.String;

    /// <summary>
    /// Indicates if the JSONValue stores a boolean value.
    /// </summary>
    public bool IsBoolean => _fieldType == JSONFieldType.Boolean;

    /// <summary>
    /// Indicates if the JSONValue has a value of null.
    /// </summary>
    public bool IsNull => _fieldType == JSONFieldType.Null;

    /// <summary>
    /// Indicates if the JSONValue stores a DateTime value.
    /// </summary>
    public bool IsDateTime => _fieldType == JSONFieldType.DateTime;

    /// <summary>
    /// Gets the value that of the JSON field, as a nullable object.
    /// </summary>
    public object? Value => _fieldType switch
    {
        JSONFieldType.Null => null,
        JSONFieldType.String => _stringValue,
        JSONFieldType.Boolean => _boolValue,
        JSONFieldType.Integer => _intValue,
        JSONFieldType.Float => _floatValue,
        JSONFieldType.List => _listValue,
        JSONFieldType.Dictionary => _dictValue,
        JSONFieldType.DateTime => _dateTimeValue,
        _ => null,
    };

    /// <summary>
    /// Gets the type of field that was set.
    /// </summary>
    public JSONFieldType FieldType => _fieldType;

    /// <summary>
    /// Compares the provided object to the current JSONValue.
    /// </summary>
    /// <param name="obj">The object to compare to the JSONValue.</param>
    /// <returns>true if the provided object is the equivalent of the JSONValue, otherwise false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (object.ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != typeof(JSONValue))
        {
            return false;
        }

        var json_obj = (JSONValue)obj;
        if (json_obj._fieldType != _fieldType)
        {
            return false;
        }

        return _fieldType switch
        {
            JSONFieldType.Null => true, // we already checked the field type and they're both Null
            JSONFieldType.String => _stringValue == json_obj._stringValue,
            JSONFieldType.Boolean => _boolValue == json_obj._boolValue,
            JSONFieldType.Integer => _intValue == json_obj._intValue,
            JSONFieldType.Float => _floatValue == json_obj._floatValue,
            JSONFieldType.DateTime => _dateTimeValue == json_obj._dateTimeValue,
            JSONFieldType.List => ListValueEquals(json_obj.ListValue),
            JSONFieldType.Dictionary => DictValueEquals(json_obj.DictValue),
            _ => throw new InvalidCastException("Unexpected JSONValue type"),
        };
    }

    /// <summary>Gets a hash code for the current object.</summary>
    public override int GetHashCode()
    {
        var typeHashCode = _fieldType.GetHashCode();
        int valueHashCode;
        switch (_fieldType)
        {
            case JSONFieldType.String:
                valueHashCode = (_stringValue ?? "").GetHashCode();
                break;
            case JSONFieldType.Boolean:
                valueHashCode = _boolValue.GetHashCode();
                break;
            case JSONFieldType.Integer:
                valueHashCode = _intValue.GetHashCode();
                break;
            case JSONFieldType.Float:
                valueHashCode = _floatValue.GetHashCode();
                break;
            case JSONFieldType.DateTime:
                valueHashCode = _dateTimeValue.GetHashCode();
                break;
            case JSONFieldType.List:
                valueHashCode = (_listValue ?? new()).GetHashCode();
                break;
            case JSONFieldType.Dictionary:
                valueHashCode = (_dictValue ?? new()).GetHashCode();
                break;
            case JSONFieldType.Null:
                valueHashCode = 0;
                break;
            default:
                throw new InvalidCastException("Unexpected JSONValue type");
        };
        return typeHashCode ^ valueHashCode;
    }

    /// <summary>
    /// Determines if the provided list equals the List Value stored in this JSONValue.
    /// </summary>
    /// <param name="value">The value to compare to our ListValue.</param>
    /// <returns>true if the two lists are equal/equivalent, otherwise false.</returns>
    private bool ListValueEquals(List<JSONValue> value)
    {
        if (object.ReferenceEquals(this._listValue, value))
        {
            return true;
        }

        if (_fieldType != JSONFieldType.List)
        {
            return false;
        }

        var compare = ListValue;
        if (compare.Count != value.Count)
        {
            return false;
        }

        for (int idx = 0; idx < compare.Count; ++idx)
        {
            if (!compare[idx].Equals(value[idx]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines if the provided dictionary is equal/equivalent to the dictionary
    /// stored in this JSONValue.
    /// </summary>
    /// <param name="value">The dictionary to compare to this JSONValue's dictionary.</param>
    /// <returns>true if the two dictionaries are equal/equivalent, otherwise false.</returns>
    private bool DictValueEquals(Dictionary<string, JSONValue> value)
    {
        if (_dictValue == null)
        {
            return false;
        }

        if (object.ReferenceEquals(_dictValue, value))
        {
            return true;
        }

        if (_fieldType != JSONFieldType.Dictionary)
        {
            return false;
        }

        var compare = DictValue;
        foreach (var keyValue in value)
        {
            if (!compare.ContainsKey(keyValue.Key))
            {
                return false;
            }

            if (!compare[keyValue.Key].Equals(keyValue.Value))
            {
                return false;
            }
        }

        foreach (var keyValue in _dictValue)
        {
            if (!value.ContainsKey(keyValue.Key))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Converts the JSONValue to a string.
    /// </summary>
    /// <returns>A string representing the JSONValue.</returns>
    /// <exception cref="InvalidCastException">Raised when an invalid JSONFieldType is assigned to the JSONValue.</exception>
    public override string ToString()
    {
        var value = Value;
        if (value == null)
        {
            return "null";
        }

        return FieldType switch
        {
            JSONFieldType.Null => "null",
            JSONFieldType.String => (string)value,
            JSONFieldType.Boolean => (bool)value ? "true" : "false",
            JSONFieldType.Float => ((double)value).ToString(),
            JSONFieldType.Integer => ((long)value).ToString(),
            JSONFieldType.DateTime => ((DateTime)value).ToString("o"), // ISO 8601 format
            JSONFieldType.List => GetListValueString((List<JSONValue>)value),
            JSONFieldType.Dictionary => GetDictValueString((Dictionary<string, JSONValue>)value),
            _ => throw new InvalidCastException("Unexpected JSONValue type"),
        };
    }

    /// <summary>
    /// Gets the value of the list as a string, in JSON format.
    /// </summary>
    /// <param name="value">The list value to convert to a string.</param>
    /// <returns>A string representing the list value.</returns>
    string GetListValueString(List<JSONValue> value)
    {
        StringBuilder builder = new();
        builder.Append('[');
        var isFirstItem = true;
        foreach (var item in value)
        {
            if (!isFirstItem)
            {
                builder.Append(", ");
            }

            isFirstItem = false;
            if (item.FieldType == JSONFieldType.String)
            {
                builder.Append($"\"{item.ToString()}\"");
            }
            else
            {
                builder.Append(item.ToString());
            }
        }

        builder.Append(']');
        return builder.ToString();
    }

    /// <summary>
    /// Gets the value of the dict as a string, in JSON format.
    /// </summary>
    /// <param name="value">The dict value to convert to a string.</param>
    /// <returns>A string representing the dict value.</returns>
    string GetDictValueString(Dictionary<string, JSONValue> value)
    {
        bool isFirstItem = true;

        StringBuilder builder = new();
        builder.Append('{');
        foreach (var item in value)
        {
            if (!isFirstItem)
            {
                builder.Append(", ");
            }

            isFirstItem = false;
            if (item.Value.FieldType == JSONFieldType.String)
            {
                // wrap the value in quotes.
                builder.Append($"\"{item.Key}\": \"{item.Value.ToString()}\"");
            }
            else
            {
                builder.Append($"\"{item.Key}\": {item.Value.ToString()}");
            }
        }
        builder.Append('}');
        return builder.ToString();
    }
}
