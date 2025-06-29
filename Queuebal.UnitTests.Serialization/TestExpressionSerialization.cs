using System.Text.Json;

using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Json;
using Queuebal.Serialization;

namespace Queuebal.UnitTests.Serialization;


[TestClass]
public class TestExpressionSerialization
{

    [TestMethod]
    public void test_serialize_expression_when_expression_is_condition_expression()
    {
        var expressionTypeRegistry = TypeRegistryService<IExpression>.BuildFromCurrentAppDomain("ExpressionType");
        var conditionTypeRegistry = TypeRegistryService<ICondition>.BuildFromCurrentAppDomain("ConditionType");
        var typeResolver = new CompositeTypeResolver()
            .AddTypeRegistry(expressionTypeRegistry)
            .AddTypeRegistry(conditionTypeRegistry);

        var condition = new IsNullCondition
        {
            NegateResult = true,
        };

        var json = JsonSerializer.Serialize<IExpression>(
            new ConditionExpression
            {
                Condition = new ConditionSet
                {
                    Conditions = [condition],
                    Operator = ConditionSetOperator.And
                },
            },
            options: new JsonSerializerOptions
            {
                TypeInfoResolver = typeResolver,
            }
        );

        var deserialized = JsonSerializer.Deserialize<IExpression>(
            json,
            options: new JsonSerializerOptions
            {
                TypeInfoResolver = typeResolver,
            }
        );

        Assert.IsNotNull(deserialized);
        Assert.IsInstanceOfType<ConditionExpression>(deserialized);

        var result = deserialized.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue("test"));
        Assert.IsTrue(result.BooleanValue); // true because NegateResult is true and inputValue is not null
    }
}
