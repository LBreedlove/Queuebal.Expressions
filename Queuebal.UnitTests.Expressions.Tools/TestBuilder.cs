#pragma warning disable CS8981, IDE0005

using Queuebal.Expressions;
using Queuebal.Services;
using Queuebal.Serialization;

using q = Queuebal.Expressions.Tools.Builder;
using System.Text.Json;

namespace Queuebal.UnitTests.Expressions.Tools;

[TestClass]
public class TestBuilder
{
    [TestMethod]
    public void test_build_sample_expression_dict()
    {
        var expression = q.dict(
            new q.kv
            {
                k = "event",
                v = q.dict(
                    new q.kv
                    {
                        k = "id",
                        v = q.Mutations.join(".", q.list(q.val("order"), q.ds("order.status"), q.ds("order.id")))
                    },
                    new q.kv
                    {
                        k = "type",
                        v = q.Mutations.join(".", q.val("order"), q.ds("order.status"))
                    },
                    new q.kv { k = "timestamp", v = q.ds("order.created_at") },
                    new q.kv { k = "source", v = q.val("order_service") },
                    new q.kv { k = "schema_version", v = q.val(123) },
                    new q.kv { k = "redacted", v = q.val(false) },
                    new q.kv { k = "ingest_discount", v = q.val(3.14) },
                    new q.kv
                    {
                        k = "data",
                        v = q.dict
                        (
                            new q.kv
                            {
                                k = "attributes",
                                v = q.dict
                                (
                                    new q.kv { k = "order_id", v = q.ds("order.id") },
                                    new q.kv { k = "value", v = q.ds("order.pricing.total") },
                                    new q.kv { k = "created_at", v = q.ds("order.created_at") },
                                    new q.kv { k = "updated_at", v = q.ds("order.updated_at") },
                                    new q.kv
                                    {
                                        k = "items",
                                        v = q.filter
                                        (
                                            q.Conditions.exp(
                                                q.Conditions.not_eq(q.val(true), q.ds("removed_from_cart"))
                                            ),
                                            q.ds("order.line_items")
                                        )
                                    },
                                    new q.kv
                                    {
                                        k = "ship_to",
                                        v = q.ds("order.shipping_address")
                                    }
                                )
                            },
                            new q.kv
                            {
                                k = "profile",
                                v = q.dict
                                (
                                    new q.kv
                                    {
                                        k = "first_name",
                                        v = q.ds
                                        (
                                            "[0]",
                                            q.Mutations.split(q.ds("order.customer_name"), " ")
                                        )
                                    },
                                    new q.kv
                                    {
                                        k = "last_name",
                                        v = q.ds
                                        (
                                            "[1]",
                                            q.Mutations.split(q.ds("order.customer_name"), " ")
                                        )
                                    },
                                    new q.kv
                                    {
                                        k = "full_name",
                                        v = q.ds("order.customer.name")
                                    },
                                    new q.kv
                                    {
                                        k = "email",
                                        v = q.ds("order.customer.email")
                                    },
                                    new q.kv
                                    {
                                        k = "location",
                                        v = q.ds("order.billing_address")
                                    },
                                    new q.kv
                                    {
                                        k = "custom_attributes",
                                        v = q.dyn_dict_with_input
                                        (
                                            q.ds("order.customer.tags"),
                                            new q.dyn_kv
                                            {
                                                k = q.ds("tag"),
                                                v = q.ds("value")
                                            }
                                        )
                                    }
                                )
                            }
                        )
                    }
                )
            }
        );

        var json = SerializeExpression(expression);
        var deserialized = DeserializeExpression(json);
        var deserializedJson = SerializeExpression(deserialized);
        Assert.AreEqual(json, deserializedJson);
    }

    /// <summary>
    /// Serializes the expression and returns the JSON.
    /// </summary>
    /// <param name="expression">The expression to serialize.</param>
    /// <returns>The JSON representing the serialized expression.</returns>
    private string SerializeExpression(IExpression expression)
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
        return JsonSerializer.Serialize(expression, options);
    }

    /// <summary>
    /// Deserializes the JSON representing an expression.
    /// </summary>
    /// <param name="json">The JSON to deserialize into an expression.</param>
    /// <returns>The deserialized IExpression.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the JsonSerializer.Deserialize method returns null.</exception>
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
        return JsonSerializer.Deserialize<IExpression>(json, options) ?? throw new InvalidOperationException("Failed to deserialize the expression");
    }
}

#pragma warning restore CS8981, IDE0005
