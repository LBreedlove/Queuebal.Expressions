using Queuebal.Expressions;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestMergeExpression
{
    public TestMergeExpression()
    {
        Context = new ExpressionContext(new Json.Data.DataProvider());
        SourceValue = new JSONValue("not used");
    }

    protected ExpressionContext Context { get; }
    protected JSONValue SourceValue { get; }

    [TestMethod]
    public void test_evaluate_when_lvalue_is_not_mergeable_throws_exception()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = true },
            RValue = new ValueExpression { Value = "test" }
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(Context, SourceValue));
    }

    [TestMethod]
    public void test_evaluate_when_lvalue_is_dict_and_rvalue_is_not_dict_throws()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = new Dictionary<string, JSONValue> { { "key1", "value1" } } },
            RValue = new ValueExpression { Value = "test" }
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(Context, SourceValue));
    }

    [TestMethod]
    public void test_evaluate_when_lvalue_is_list_and_rvalue_is_not_list_throws()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = new List<JSONValue> { "value1" } },
            RValue = new ValueExpression { Value = "test" }
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(Context, SourceValue));
    }

    [TestMethod]
    public void test_evaluate_when_lvalue_is_string_and_rvalue_is_not_string_throws()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = "Hello, " },
            RValue = new ValueExpression { Value = 123 }
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(Context, SourceValue));
    }

    [TestMethod]
    public void test_evaluate_when_lvalue_is_number_and_rvalue_is_not_number_throws()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = 100 },
            RValue = new ValueExpression { Value = "test" }
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(Context, SourceValue));
    }

    [TestMethod]
    public void test_evaluate_when_both_values_are_dicts_merges_them()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = new Dictionary<string, JSONValue> { { "key1", "value1" } } },
            RValue = new ValueExpression { Value = new Dictionary<string, JSONValue> { { "key2", "value2" } } }
        };

        // Act
        var result = expression.Evaluate(Context, SourceValue);

        // Assert
        Assert.IsTrue(result.IsDict);
        Assert.AreEqual(2, result.DictValue.Count);
        Assert.AreEqual("value1", result.DictValue["key1"].StringValue);
        Assert.AreEqual("value2", result.DictValue["key2"].StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_both_values_are_lists_merges_them()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = new List<JSONValue> { "value1" } },
            RValue = new ValueExpression { Value = new List<JSONValue> { "value2" } }
        };

        // Act
        var result = expression.Evaluate(Context, SourceValue);

        // Assert
        Assert.IsTrue(result.IsList);
        Assert.AreEqual(2, result.ListValue.Count);
        Assert.AreEqual("value1", result.ListValue[0].StringValue);
        Assert.AreEqual("value2", result.ListValue[1].StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_both_values_are_strings_concatenates_them()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = "Hello, " },
            RValue = new ValueExpression { Value = "World!" }
        };

        // Act
        var result = expression.Evaluate(Context, SourceValue);

        // Assert
        Assert.IsTrue(result.IsString);
        Assert.AreEqual("Hello, World!", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_both_values_are_ints_adds_them()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = 100 },
            RValue = new ValueExpression { Value = 50 }
        };

        // Act
        var result = expression.Evaluate(Context, SourceValue);

        // Assert
        Assert.IsTrue(result.IsNumber);
        Assert.AreEqual(150, result.IntValue);
    }

    [TestMethod]
    public void test_evaluate_when_both_values_are_floats_adds_them()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = 100.5 },
            RValue = new ValueExpression { Value = 50.25 }
        };

        // Act
        var result = expression.Evaluate(Context, SourceValue);

        // Assert
        Assert.IsTrue(result.IsNumber);
        Assert.AreEqual(150.75, result.FloatValue);
    }

    [TestMethod]
    public void test_evaluate_when_lvalue_is_int_and_rvalue_is_float_adds_them()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = 100 },
            RValue = new ValueExpression { Value = 50.5 }
        };

        // Act
        var result = expression.Evaluate(Context, SourceValue);

        // Assert
        Assert.IsTrue(result.IsNumber);
        Assert.AreEqual(150.5, result.FloatValue);
    }


    [TestMethod]
    public void test_evaluate_when_lvalue_is_float_and_rvalue_is_int_adds_them()
    {
        // Arrange
        var expression = new MergeExpression
        {
            LValue = new ValueExpression { Value = 50.5 },
            RValue = new ValueExpression { Value = 100 },
        };

        // Act
        var result = expression.Evaluate(Context, SourceValue);

        // Assert
        Assert.IsTrue(result.IsNumber);
        Assert.AreEqual(150.5, result.FloatValue);
    }
}