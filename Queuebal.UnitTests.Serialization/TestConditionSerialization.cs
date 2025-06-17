using System.Text.Json;

using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Services;
using Queuebal.Serialization;

namespace Queuebal.UnitTests.Serialization;

[TestClass]
public class TestConditionSerialization
{
    [TestMethod]
    public void test_serialize_condition_when_condition_is_is_null_condition()
    {
        var typeRegistry = TypeRegistryService<ICondition>.BuildFromCurrentAppDomain("ConditionType");
        var typeResolver = new TypeResolver<ICondition>(typeRegistry);

        var json = JsonSerializer.Serialize<ICondition>(
            new IsNullCondition(),
            options: new JsonSerializerOptions
            {
                TypeInfoResolver = typeResolver,
            }
        );

        var deserialized = JsonSerializer.Deserialize<ICondition>(
            json,
            options: new JsonSerializerOptions
            {
                TypeInfoResolver = typeResolver,
            }
        );

        Assert.IsNotNull(deserialized);
        Assert.IsInstanceOfType<IsNullCondition>(deserialized);
    }
}