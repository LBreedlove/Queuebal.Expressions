using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.UnitTests.Expressions;


[TestClass]
public class TestProjectionExpression
{
    [TestMethod]
    public void test_evaluate_when_include_data_from_parent_object()
    {
        var expression = BuildProjectionExpression();
        var inputValue = BuildInputValue();
        var filter = BuildFilterCondition();
        expression.ItemFilter = filter;
       
        var context = new ExpressionContext(new DataProvider());
        var result = expression.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsList);
        Assert.AreEqual(1, result.ListValue.Count);

        // only item1 passed through the filter
        Assert.AreEqual("item1", result.ListValue[0].DictValue["item"].StringValue);
        Assert.AreEqual("parentValue", result.ListValue[0].DictValue["parent"].StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_value_is_not_list_converts_to_list()
    {
        var expression = BuildNonListProjectionExpression();
        var inputValue = BuildInputValue();

        var context = new ExpressionContext(new DataProvider());
        var result = expression.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsList);
        Assert.AreEqual(1, result.ListValue.Count);

        Assert.AreEqual("parentValue", result.ListValue[0].DictValue["parent"].StringValue);
        Assert.IsTrue(result.ListValue[0].DictValue.ContainsKey("item"));

        var expectedItem = BuildInputValue()["simple"].DictValue["path"];
        Assert.AreEqual(result.ListValue[0].DictValue["item"], expectedItem);
    }

    [TestMethod]
    public void test_evaluate_when_items_is_null_returns_null()
    {
        var expression = BuildMissingProjectionExpression();
        var inputValue = BuildInputValue();

        var context = new ExpressionContext(new DataProvider());
        var result = expression.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsNull);
    }

    private ConditionExpression BuildFilterCondition()
    {
        return new ConditionExpression
        {
            ConditionSet = new ConditionSet
            {
                Conditions = [
                    new EqualsCondition
                    {
                        ComparerValueExpression = new ValueExpression { Value = new JSONValue("item1") }
                    }
                ]
            }
        };
    }

    private Dictionary<string, JSONValue> BuildInputValue() =>
        new Dictionary<string, JSONValue>
        {
            { "simple", new Dictionary<string, JSONValue>
                {
                    { "parent", new JSONValue("parentValue") },
                    { "path", new Dictionary<string, JSONValue>
                        {
                            { "items", new List<JSONValue>
                                {
                                    new JSONValue("item1"),
                                    new JSONValue("item2"),
                                }
                            }
                        }
                    }
                }
            }
        };

    private ProjectionExpression BuildProjectionExpression()
    {
        return new ProjectionExpression
        {
            Items = new DataSelectorExpression
            {
                Path = "simple.path.items[:]"
            },
            ContextDataMap = new Dictionary<string, Expression>
            {
                { "parent", new DataSelectorExpression
                    {
                        Path = "simple.parent"
                    }
                }
            },
            Projection = new Dictionary<string, Expression>
            {
                { "item", new DataSelectorExpression
                    {
                        Path = "item"
                    }
                },
                { "parent", new DataSelectorExpression
                    {
                        Path = "parent"
                    }
                }
            },
        };
    }

    private ProjectionExpression BuildNonListProjectionExpression()
    {
        return new ProjectionExpression
        {
            Items = new DataSelectorExpression
            {
                Path = "simple.path"
            },
            ContextDataMap = new Dictionary<string, Expression>
            {
                { "parent", new DataSelectorExpression
                    {
                        Path = "simple.parent"
                    }
                }
            },
            Projection = new Dictionary<string, Expression>
            {
                { "item", new DataSelectorExpression
                    {
                        Path = "item"
                    }
                },
                { "parent", new DataSelectorExpression
                    {
                        Path = "parent"
                    }
                }
            },
        };
    }

    private ProjectionExpression BuildMissingProjectionExpression()
    {
        return new ProjectionExpression
        {
            Items = new DataSelectorExpression
            {
                Path = "invalid.path.items[:]"
            },
            ContextDataMap = new Dictionary<string, Expression>
            {
                { "parent", new DataSelectorExpression
                    {
                        Path = "simple.parent"
                    }
                }
            },
            Projection = new Dictionary<string, Expression>
            {
                { "item", new DataSelectorExpression
                    {
                        Path = "item"
                    }
                },
                { "parent", new DataSelectorExpression
                    {
                        Path = "parent"
                    }
                }
            },
        };
    }
}
