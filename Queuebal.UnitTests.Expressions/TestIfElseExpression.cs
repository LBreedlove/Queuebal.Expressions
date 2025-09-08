using Queuebal.Expressions;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions;


[TestClass]
public class TestIfElseExpression
{
    [TestMethod]
    public void test_evaluate_when_condition_is_true_returns_then_value()
    {
        // Arrange
        var expression = new IfElseExpression
        {
            Branches = new List<IfElseExpressionCondition>
            {
                new IfElseExpressionCondition
                {
                    Condition = new ConditionExpression
                    {
                        Condition = ConditionSet.AlwaysFalse,
                    },
                    IfTrue = new ValueExpression { Value = new JSONValue("Condition A - False") }
                },
                new IfElseExpressionCondition
                {
                    Condition = new ConditionExpression
                    {
                        Condition = ConditionSet.AlwaysFalse,
                    },
                    IfTrue = new ValueExpression { Value = new JSONValue("Condition B - False") }
                },
                new IfElseExpressionCondition
                {
                    Condition = new ConditionExpression
                    {
                        Condition = ConditionSet.AlwaysTrue,
                    },
                    IfTrue = new ValueExpression { Value = new JSONValue("Condition C - True") }
                }
            }
        };

        // Act
        var result = expression.Evaluate(new ExpressionContext(new Json.Data.VariableProvider()), new JSONValue());

        // Assert
        Assert.AreEqual("Condition C - True", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_no_conditions_match_and_else_value_is_provided_returns_else_value()
    {
        // Arrange
        var expression = new IfElseExpression
        {
            Branches = new List<IfElseExpressionCondition>
            {
                new IfElseExpressionCondition
                {
                    Condition = new ConditionExpression
                    {
                        Condition = ConditionSet.AlwaysFalse,
                    },
                    IfTrue = new ValueExpression { Value = new JSONValue("Condition A - False") }
                },
            },
            ElseValue = new ValueExpression { Value = new JSONValue("Else Value") }
        };

        // Act
        var result = expression.Evaluate(new ExpressionContext(new Json.Data.VariableProvider()), new JSONValue());

        // Assert
        Assert.AreEqual("Else Value", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_no_conditions_match_and_else_value_is_not_provided_returns_empty_value()
    {
        // Arrange
        var expression = new IfElseExpression
        {
            Branches = new List<IfElseExpressionCondition>
            {
                new IfElseExpressionCondition
                {
                    Condition = new ConditionExpression
                    {
                        Condition = ConditionSet.AlwaysFalse,
                    },
                    IfTrue = new ValueExpression { Value = new JSONValue("Condition A - False") }
                },
            },
            ElseValue = null
        };

        // Act
        var result = expression.Evaluate(new ExpressionContext(new Json.Data.VariableProvider()), new JSONValue());

        // Assert
        Assert.IsTrue(result.IsNull);
    }
}