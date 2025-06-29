using Queuebal.Expressions;
using Queuebal.Json;
using Queuebal.Serialization;

namespace Queuebal.UnitTests.Serialization;

[TestClass]
public class TestTypeRegistryService
{
    [TestMethod]
    public void test_ctor_when_base_type_does_not_contain_discriminator_property()
    {
        // Arrange & Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => new TypeRegistryService<IExpression>("invalidDiscriminator"));
    }

    [TestMethod]
    public void test_ctor_when_discriminator_field_is_empty()
    {
        // Arrange & Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => new TypeRegistryService<IExpression>(""));
    }

    [TestMethod]
    public void test_with_type_mapping_with_generic_type()
    {
        // Arrange
        var service = new TypeRegistryService<IExpression>("ExpressionType");

        // Act
        service.WithTypeMapping<ValueExpression>();

        // Assert
        var typeMap = service.TypeMap;
        Assert.IsTrue(typeMap.ContainsKey("Value"));
        Assert.AreEqual(typeof(ValueExpression), typeMap["Value"]);
    }

    [TestMethod]
    public void test_with_type_mapping_with_type_parameter()
    {
        // Arrange
        var service = new TypeRegistryService<IExpression>("ExpressionType");

        // Act
        service.WithTypeMapping(typeof(ValueExpression));

        // Assert
        var typeMap = service.TypeMap;
        Assert.IsTrue(typeMap.ContainsKey("Value"));
        Assert.AreEqual(typeof(ValueExpression), typeMap["Value"]);
    }

    [TestMethod]
    public void test_register_type_mapping_when_implementation_type_does_not_implement_base_type()
    {
        // Arrange
        var service = new TypeRegistryService<IExpression>("ExpressionType");

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => service.RegisterTypeMapping(typeof(string)));
    }

    [TestMethod]
    public void test_register_type_mapping_when_type_already_registered()
    {
        // Arrange
        var service = new TypeRegistryService<IExpression>("ExpressionType");
        service.RegisterTypeMapping(typeof(ValueExpression));

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => service.RegisterTypeMapping(typeof(ValueExpression)));
    }

    /// <summary>
    /// Class used for testing the TypeRegistryService.
    /// </summary>
    public class InvalidExpression : Expression
    {
        public static string ExpressionType => string.Empty;
        protected override JSONValue EvaluateExpression(ExpressionContext context, JSONValue inputValue) =>
            throw new NotImplementedException("This is an invalid expression for testing purposes.");
    }

    [TestMethod]
    public void test_register_type_mapping_when_discriminator_value_is_empty()
    {
        // Arrange
        var service = new TypeRegistryService<IExpression>("ExpressionType");

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => service.RegisterTypeMapping(typeof(InvalidExpression)));
    }
}
