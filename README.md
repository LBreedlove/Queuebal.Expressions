[![.NET](https://github.com/LBreedlove/Queuebal.Expressions/actions/workflows/dotnet.yml/badge.svg)](https://github.com/LBreedlove/Queuebal.Expressions/actions/workflows/dotnet.yml)
[![Coverage Status](https://coveralls.io/repos/github/LBreedlove/Queuebal.Expressions/badge.svg?branch=main&kill_cache=20)](https://coveralls.io/github/LBreedlove/Queuebal.Expressions?branch=main&kill_cache=20)

# Queuebal.Expressions

Queuebal.Expressions is a .NET library for building, evaluating, and composing data transformation pipelines using expressions that can be configured via JSON. It provides tools for selecting, transforming, and writing data in JSON-like structures, making it ideal for dynamic data processing and ETL scenarios.

## Features

- **Composable Expressions:** Build complex data transformations using a set of reusable, chainable expression types.
- **JSON Data Selection:** Read and write values in deeply nested JSON objects using simple, intuitive path syntax.
- **Extensible:** Easily add custom expressions or extend existing ones.

---

## Solution Structure

- **Queuebal.Expressions:** Core expression types and evaluation logic.
- **Queuebal.Expressions.Conditions:** Various condition types.
- **Queuebal.Expressions.Mutations:** Various mutation types.
- **Queuebal.Expressions.Tools:** Tools for making it easier to build expressions.
- **Queuebal.Json:** JSON value abstraction and helpers.
- **Queuebal.Json.Data:** DataSelector and DataWriter for path-based JSON access and mutation.
- **Queuebal.Serialization:** TypeConverter utility for handling polymorphic deserialization, as well as the `JSONValueConverter` for writing JSON values to a JSON serializer.
- **Queuebal.UnitTests.\***: Unit tests for all components.

---

## Expressions

Expressions are the core building blocks for data transformation. They can be composed to perform mapping, filtering, projection, and conditional logic.

**Example: MapExpression**

```csharp
var expr = new MapExpression
{
    Map = new ValueExpression { Value = new JSONValue("test") }
};
var context = new ExpressionContext(new DataProvider());
var result = expr.Evaluate(context, new JSONValue(new List<JSONValue> { "a", "b" }));
// result: ["test", "test"]
```

Expressions can be configured via JSON, making them easy to use in dynamic scenarios.

---

## DataSelector

`DataSelector` allows you to read values from a JSON object using a simple path syntax.

**Example:**

```csharp
var selector = new DataSelector("items[:].name");
var results = selector.GetValues(jsonValue);
// Returns all 'name' fields in the 'items' array
```

- Supports object properties, array indices, and slices.
- Returns a list of results, each with the value and its path.

---

## DataWriter

`DataWriter` enables writing values into a JSON object at a specified path.

**Example:**

```csharp
var writer = new DataWriter();
var updated = writer.WriteValue("items[0].name", new JSONValue("new name"));
// Updates the 'name' field of the first item in 'items'
```

- Automatically creates intermediate objects or arrays as needed.
- Throws exceptions if the path is incompatible with the structure.

---

## Usage Example

Suppose you have the following JSON:

```json
{
  "items": [
    { "name": "item1" },
    { "name": "item2" }
  ]
}
```

To select all item names:

```csharp
var selector = new DataSelector("items[:].name");
var names = selector.GetValues(jsonValue).Select(r => r.Value);
```

To update the first item's name:

```csharp
var writer = new DataWriter();
var updated = writer.WriteValue("items[0].name", new JSONValue("updatedName"));
```

To transform all names using an expression:

```csharp
var expr = new MapExpression
{
    Map = new ValueExpression { Value = new JSONValue("newName") }
};
var context = new ExpressionContext(new DataProvider());
var result = expr.Evaluate(context, jsonValue["items"]);
```

---

## Running Tests

To run the unit tests:

```sh
dotnet test
```

---

## Serialization
To support polymorphic deserialization, you will need to register the available `IExpression` types along with their `ExpressionType` discriminator value. The `TypeRegistryService` can handle this for you. You will also need to setup a `TypeResolver`; in order to support deserializing multiple types of polymorphic values, the `Serialization` project provides a `CompositeTypeResolver` that lets you register multiple base types to deserialize.
You will also need to register a `JsonConverter` to deserialize the `JSONValue` type. The example below demonstrates how you can setup your `JsonSerializerOptions` to support this.

```csharp
private IExpression DeserializeExpression(string json)
{
    // build the type registry used to serialize the expressions
    var expressionTypeRegistry = TypeRegistryService<IExpression>.BuildFromCurrentAppDomain("ExpressionType");
    var conditionTypeRegistry = TypeRegistryService<ICondition>.BuildFromCurrentAppDomain("ConditionType");
    var mutationTypeRegistry = TypeRegistryService<IMutation>.BuildFromCurrentAppDomain("MutationType");

    var typeResolver = new CompositeTypeResolver()
        .AddTypeRegistry(expressionTypeRegistry)
        .AddTypeRegistry(conditionTypeRegistry)
        .AddTypeRegistry(mutationTypeRegistry);

    var options = new JsonSerializerOptions
    {
        TypeInfoResolver = typeResolver,
    };

    options.Converters.Add(new JSONValueConverter());
    return JsonSerializer.Deserialize<IExpression>(json, options);
}
```

---

## What's Left
* Add new `Mutation` types
* Add new `Condition` types
* Add remaining mutations to the `Builder`.
* Increase code coverage of `JSONValue`, `JSONValueConverter`.
* Add more examples to `Queuebal.UnitTests.Examples`.
* Improve the README to provide examples:
  * `DataWriter` and `DataSelector` path format.
  * Examples of chained expressions.
* Publish a NuGet package.

## Extending

You can add new expression types by deriving from the `Expression` class; new expression types can be auto-added to the `TypeRegistryService` if the type registry is built using `BuildFromCurrentAppDomain`.

New `Condition` types should be derived from `BinaryCondition` or `UnaryCondition`, depending on whether or not the Condition requires an input value (comparer).

New `Mutation` types should be derived from `Mutation`. The `Mutation` base class will handle evaluating the input expression, and replacing placeholders with variable values when the mutation result is a `string`.

---

For more details, see the source code and unit tests in the respective project folders.

Note: This README was written by ChatGPT.
