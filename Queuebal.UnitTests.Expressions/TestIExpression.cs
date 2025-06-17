using Queuebal.Expressions;

namespace Queuebal.UnitTests.Expressions;


[TestClass]
public class TestIExpression
{
    [TestMethod]
    public void test_get_expression_type_throws_exception()
    {
        // Arrange & Act & Assert
        Assert.ThrowsException<NotImplementedException>(() => IExpression.ExpressionType);
    }
}