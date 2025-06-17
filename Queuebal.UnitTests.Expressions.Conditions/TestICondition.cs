using Queuebal.Expressions;

namespace Queuebal.UnitTests.Expressions.Conditions;


[TestClass]
public class TestICondition
{
    [TestMethod]
    public void test_get_condition_type_throws_exception()
    {
        // Arrange & Act & Assert
        Assert.ThrowsException<NotImplementedException>(() => ICondition.ConditionType);
    }
}
