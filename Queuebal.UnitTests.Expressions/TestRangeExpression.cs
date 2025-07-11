using Queuebal.Expressions;

namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestRangeExpression
{
    [TestMethod]
    public void test_evaluate_when_start_is_negative()
    {
        // Arrange
        var expression = new RangeExpression
        {
            Start = new ValueExpression { Value = -1 },
            Count = new ValueExpression { Value = 5 },
        };

        var context = new ExpressionContext(new());

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(context, new()));
    }

    [TestMethod]
    public void test_evaluate_when_start_is_not_number_throws()
    {
        // Arrange
        var expression = new RangeExpression
        {
            Start = new ValueExpression { Value = "hello world" },
            Count = new ValueExpression { Value = 5 },
        };

        var context = new ExpressionContext(new());

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(context, new()));
    }

    [TestMethod]
    public void test_evaluate_when_count_is_negative()
    {
        // Arrange
        var expression = new RangeExpression
        {
            Start = new ValueExpression { Value = 0 },
            Count = new ValueExpression { Value = -5 },
        };
        var context = new ExpressionContext(new());

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(context, new()));
    }

    [TestMethod]
    public void test_evaluate_when_count_is_not_number_throws()
    {
        // Arrange
        var expression = new RangeExpression
        {
            Start = new ValueExpression { Value = 0 },
            Count = new ValueExpression { Value = "hello world" },
        };

        var context = new ExpressionContext(new());

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(context, new()));
    }

    [TestMethod]
    public void test_evaluate_when_step_is_1()
    {
        // Arrange
        var expression = new RangeExpression
        {
            Start = new ValueExpression { Value = 0 },
            Count = new ValueExpression { Value = 5 },
        };

        var context = new ExpressionContext(new());

        // Act
        var result = expression.Evaluate(context, new());

        // Assert
        Assert.AreEqual(5, result.ListValue.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(i, result.ListValue[i].IntValue);
        }
    }

    [TestMethod]
    public void test_evaluate_when_step_is_2()
    {
        // Arrange
        var expression = new RangeExpression
        {
            Start = new ValueExpression { Value = 0 },
            Count = new ValueExpression { Value = 5 },
            Step = new ValueExpression { Value = 2 },
        };
        var context = new ExpressionContext(new());

        // Act
        var result = expression.Evaluate(context, new());

        // Assert
        Assert.AreEqual(5, result.ListValue.Count);
        Assert.AreEqual(0, result.ListValue[0].IntValue);
        Assert.AreEqual(2, result.ListValue[1].IntValue);
        Assert.AreEqual(4, result.ListValue[2].IntValue);
        Assert.AreEqual(6, result.ListValue[3].IntValue);
        Assert.AreEqual(8, result.ListValue[4].IntValue);
    }


    [TestMethod]
    public void test_evaluate_when_step_is_not_number_throws()
    {
        // Arrange
        var expression = new RangeExpression
        {
            Start = new ValueExpression { Value = 0 },
            Count = new ValueExpression { Value = 5 },
            Step = new ValueExpression { Value = "hello world" },
        };
        var context = new ExpressionContext(new());

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(context, new()));
    }
}