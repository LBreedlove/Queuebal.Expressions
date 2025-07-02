using Queuebal.Expressions;
using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.UnitTests.Expressions;


[TestClass]
public class TestDeclareAndAssignExpression
{
    [TestMethod]
    public void test_evaluate_when_variable_already_exists_throws()
    {
        // Arrange
        var context = new ExpressionContext(new DataProvider());
        context.DataProvider.AddValue("existingVar", new JSONValue(42));

        var expression = new DeclareAndAssignExpression
        {
            VariableName = "existingVar",
            Value = new ValueExpression { Value = new JSONValue(100) }
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(context, new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_variable_does_not_exist_creates_and_assigns_value()
    {
        // Arrange
        var context = new ExpressionContext(new DataProvider());
        var expression = new DeclareAndAssignExpression
        {
            VariableName = "newVar",
            Value = new ValueExpression { Value = new JSONValue(100) }
        };
        // Act
        var result = expression.Evaluate(context, new JSONValue());
        // Assert
        Assert.AreEqual(new JSONValue(100), result);
        Assert.AreEqual(new JSONValue(100), context.DataProvider.GetValue("newVar"));
    }
}