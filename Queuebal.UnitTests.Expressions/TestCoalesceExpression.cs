using Queuebal.Json;
using Queuebal.Json.Data;
using Queuebal.Expressions;

namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestCoalesceExpression
{
    public TestCoalesceExpression()
    {
        Context = new ExpressionContext(new VariableProvider());
    }

    /// <summary>
    /// The ExpressionContext used by the tests.
    /// </summary>
    protected ExpressionContext Context { get; }

    [TestMethod]
    public void test_evaluate_when_all_values_return_null_returns_null()
    {
        // Arrange
// Arrange
        var inputValue = new Dictionary<string, JSONValue>
        {
            { "simple", new Dictionary<string, JSONValue>
            {
                { "path", new Dictionary<string, JSONValue>
                    {
                        { "null_value", new() },
                        { "value", "An existing value" },
                        { "items", new List<JSONValue>
                            {
                                "hello", "world"
                            }
                        }
                    }
                }
            }}
        };

        var expression = new CoalesceExpression
        {
            Values = new List<IExpression>
            {
                new DataSelectorExpression { Path = "simple.path.null_value" },
                new DataSelectorExpression { Path = "simple.path.wrong_key" },
            }
        };

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        Assert.IsTrue(result.IsNull);
    }

    [TestMethod]
    public void test_evaluate_returns_first_non_null_value()
    {
        // Arrange
        var inputValue = new Dictionary<string, JSONValue>
        {
            { "simple", new Dictionary<string, JSONValue>
            {
                { "path", new Dictionary<string, JSONValue>
                    {
                        { "null_value", new() },
                        { "value", "An existing value" },
                        { "items", new List<JSONValue>
                            {
                                "hello", "world"
                            }
                        }
                    }
                }
            }}
        };

        var expression = new CoalesceExpression
        {
            Values = new List<IExpression>
            {
                new DataSelectorExpression { Path = "simple.path.null_value" },
                new DataSelectorExpression { Path = "simple.path.value" },
                new DataSelectorExpression { Path = "simple.path.items" },
            }
        };

        // Act
        var result = expression.Evaluate(Context, inputValue);

        // Assert
        Assert.AreEqual("An existing value", result.StringValue);
    }
}
