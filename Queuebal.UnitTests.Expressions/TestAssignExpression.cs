using Queuebal.Expressions;
using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.UnitTests.Expressions;


[TestClass]
public class TestAssignExpression
{
    [TestMethod]
    public void test_evaluate_when_variable_already_exists_sets_value()
    {
        // Arrange
        var context = new ExpressionContext(new VariableProvider());
        context.VariableProvider.AddValue("existingVar", new JSONValue(42));

        var expression = new AssignExpression
        {
            VariableName = "existingVar",
            Value = new ValueExpression { Value = new JSONValue(100) }
        };

        // Act & Assert
        var result = expression.Evaluate(context, new JSONValue());
        Assert.AreEqual(100, result.IntValue);
    }

    [TestMethod]
    public void test_evaluate_when_variable_does_not_exist_throws()
    {
        // Arrange
        var context = new ExpressionContext(new VariableProvider());
        var expression = new AssignExpression
        {
            VariableName = "newVar",
            Value = new ValueExpression { Value = new JSONValue(100) }
        };
        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(context, new JSONValue()));
    }
}