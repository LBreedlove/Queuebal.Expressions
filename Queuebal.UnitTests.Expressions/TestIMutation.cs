using Queuebal.Expressions;

namespace Queuebal.UnitTests.Expressions;


[TestClass]
public class TestIMutation
{
    [TestMethod]
    public void test_get_mutation_type_throws_exception()
    {
        // Arrange & Act & Assert
        Assert.ThrowsException<NotImplementedException>(() => IMutation.MutationType);
    }
}