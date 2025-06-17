using System.Text.Json;

using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Services;
using Queuebal.Serialization;

namespace Queuebal.UnitTests.Serialization;

[TestClass]
public class TestMutationSerialization
{
    [TestMethod]
    public void test_serialize_mutation_when_mutation_is_to_date_time_mutation()
    {
        var typeRegistry = TypeRegistryService<IMutation>.BuildFromCurrentAppDomain("MutationType");
        var typeResolver = new TypeResolver<IMutation>(typeRegistry);

        var json = JsonSerializer.Serialize<IMutation>(
            new ToDateTimeMutation(),
            options: new JsonSerializerOptions
            {
                TypeInfoResolver = typeResolver,
            }
        );

        var deserialized = JsonSerializer.Deserialize<IMutation>(
            json,
            options: new JsonSerializerOptions
            {
                TypeInfoResolver = typeResolver,
            }
        );

        Assert.IsNotNull(deserialized);
        Assert.IsInstanceOfType<ToDateTimeMutation>(deserialized);
    }
}