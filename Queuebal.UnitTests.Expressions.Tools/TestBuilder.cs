#pragma warning disable CS8981, IDE0005

using Queuebal.Expressions;
using Queuebal.Json;
using Queuebal.Serialization;

using q = Queuebal.Expressions.Tools.Builder;
using System.Text.Json;
using Queuebal.Expressions.Conditions;
using System.Net.Http.Headers;

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

    [TestMethod]
    public void test_dict_with_input_builds_dict_expression()
    {
        var expression = q.dict_with_input(q.val(123));
        Assert.IsInstanceOfType<DictExpression>(expression);
        Assert.IsNotNull(expression.InputValue);
    }

    [TestMethod]
    public void test_list_with_input_builds_list_expression()
    {
        var expression = q.list_with_input(q.val(123));
        Assert.IsInstanceOfType<ListExpression>(expression);
        Assert.IsNotNull(expression.InputValue);
    }

    [TestMethod]
    public void test_dyn_dict_builds_dict_expression()
    {
        var expression = q.dyn_dict();
        Assert.IsInstanceOfType<DynamicDictExpression>(expression);
        Assert.IsNull(expression.InputValue);
    }

    [TestMethod]
    public void test_conditions_all_builds_all_condition()
    {
        var condition = q.Conditions.all
        (
            new EqualsCondition { ComparerValue = q.val(123) },
            new IsNullCondition { NegateResult = true }
        );

        var result = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 123);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_conditions_any_builds_any_of_condition()
    {
        var condition = q.Conditions.any
        (
            new EqualsCondition { ComparerValue = q.val(123) },
            new IsNullCondition()
        );

        var result = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue());
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_conditions_none_of_builds_none_of_condition()
    {
        var condition = q.Conditions.none
        (
            new EqualsCondition { ComparerValue = q.val(123) },
            new IsNullCondition()
        );

        var result = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 456);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_eq_builds_equals_condition()
    {
        var condition = q.Conditions.eq(q.val(123));
        Assert.IsInstanceOfType<EqualsCondition>(condition);
    }

    [TestMethod]
    public void test_not_eq_builds_equals_condition_with_negate_result_true()
    {
        var condition = q.Conditions.not_eq(q.val(123));
        Assert.IsInstanceOfType<EqualsCondition>(condition);
        var result = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 456);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_is_null_builds_is_null_condition()
    {
        var condition = q.Conditions.is_null();
        Assert.IsInstanceOfType<IsNullCondition>(condition);
        var result = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue());
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void test_not_null_builds_is_null_condition_with_negate_result_true()
    {
        var condition = q.Conditions.not_null();
        Assert.IsInstanceOfType<IsNullCondition>(condition);
        var result = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue());
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void test_is_greater_than_builds_is_greater_than_condition()
    {
        var condition = q.Conditions.is_greater_than(q.val(37));
        Assert.IsInstanceOfType<GreaterThanCondition>(condition);

        var result1 = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 37);
        Assert.IsFalse(result1);

        var result2 = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 38);
        Assert.IsTrue(result2);
    }

    [TestMethod]
    public void test_is_greater_than_or_equal_builds_is_greater_than_or_equal_condition()
    {
        var condition = q.Conditions.is_greater_than_or_equal(q.val(37));
        Assert.IsInstanceOfType<GreaterThanOrEqualCondition>(condition);

        var result1 = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 37);
        Assert.IsTrue(result1);

        var result2 = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 38);
        Assert.IsTrue(result2);
    }

    [TestMethod]
    public void test_is_less_than_builds_is_greater_than_condition()
    {
        var condition = q.Conditions.is_less_than(q.val(37));
        Assert.IsInstanceOfType<LessThanCondition>(condition);

        var result1 = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 37);
        Assert.IsFalse(result1);

        var result2 = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 36);
        Assert.IsTrue(result2);
    }

    [TestMethod]
    public void test_is_less_than_or_equal_builds_is_greater_than_or_equal_condition()
    {
        var condition = q.Conditions.is_less_than_or_equal(q.val(37));
        Assert.IsInstanceOfType<LessThanOrEqualCondition>(condition);

        var result1 = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 37);
        Assert.IsTrue(result1);

        var result2 = condition.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), 36);
        Assert.IsTrue(result2);
    }

    [TestMethod]
    public void test_is_length_greater_than_builds_is_length_greater_than_condition()
    {
        var condition = q.Conditions.is_length_greater_than(new ValueExpression { Value = new List<JSONValue> { 123 } });
        Assert.IsInstanceOfType<LengthIsGreaterThanCondition>(condition);
    }

    [TestMethod]
    public void test_is_length_greater_than_or_equal_builds_is_length_greater_than_or_equal_condition()
    {
        var condition = q.Conditions.is_length_greater_than_or_equal(new ValueExpression { Value = new List<JSONValue> { 123 } });
        Assert.IsInstanceOfType<LengthIsGreaterThanOrEqualCondition>(condition);
    }

    [TestMethod]
    public void test_is_length_less_than_builds_is_length_less_than_condition()
    {
        var condition = q.Conditions.is_length_less_than(new ValueExpression { Value = new List<JSONValue> { 123 } });
        Assert.IsInstanceOfType<LengthIsLessThanCondition>(condition);
    }

    [TestMethod]
    public void test_is_length_less_than_or_equal_builds_is_length_less_than_or_equal_condition()
    {
        var condition = q.Conditions.is_length_less_than_or_equal(new ValueExpression { Value = new List<JSONValue> { 123 } });
        Assert.IsInstanceOfType<LengthIsLessThanOrEqualCondition>(condition);
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
