using System.IO.Pipelines;

using Queuebal.Expressions;
using Queuebal.Expressions.Mutations;
using Queuebal.Json;

namespace Queuebal.UnitTests.Expressions.Mutations;


[TestClass]
public class TestCullMutation
{
    [TestMethod]
    public void test_evaluate_when_input_is_not_dict_or_list_returns_value()
    {
        var mutation = new CullMutation
        {
            MaxDepth = new ValueExpression { Value = new JSONValue(1) }
        };
        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());
        var inputValue = new JSONValue("not a dict");
        var result = mutation.Evaluate(context, inputValue);
        Assert.AreEqual(inputValue, result);
    }

    [TestMethod]
    public void test_evaluate_when_depth_is_not_number_throws_exception()
    {
        var mutation = new CullMutation
        {
            MaxDepth = new ValueExpression { Value = new JSONValue("not a number") }
        };
        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());
        var inputValue = new JSONValue(new Dictionary<string, JSONValue>());
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_value_is_null_returns_null()
    {
        var mutation = new CullMutation
        {
            MaxDepth = new ValueExpression { Value = new JSONValue(1) }
        };
        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());
        var inputValue = new JSONValue();
        var result = mutation.Evaluate(context, inputValue);
        Assert.IsTrue(result.IsNull);
    }

    [TestMethod]
    public void test_when_input_is_list_culls_dict_values()
    {
        var mutation = new CullMutation
        {
            MaxDepth = new ValueExpression { Value = new JSONValue(1) }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());

        var inputValue = new JSONValue(new List<JSONValue>
        {
            new Dictionary<string, JSONValue>
            {
                { "key1", "value1" },
                { "key2", new Dictionary<string, JSONValue>
                    {
                        { "subkey1", "subvalue1" },
                        { "subkey2", new Dictionary<string, JSONValue>
                            {
                                { "subsubkey1", "subsubvalue1" }
                            }
                        }
                    }
                }
            }
        });

        var result = mutation.Evaluate(context, inputValue);
        var expectedListItem = new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", new JSONValue(new Dictionary<string, JSONValue>()) }
        };

        Assert.AreEqual(1, result.ListValue.Count);
        CollectionAssert.AreEquivalent(expectedListItem, result.ListValue[0].DictValue);
    }

    [TestMethod]
    public void test_evaluate_when_depth_is_less_than_1_throws_exception()
    {
        var mutation = new CullMutation
        {
            MaxDepth = new ValueExpression { Value = new JSONValue(0) }
        };
        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());
        var inputValue = new JSONValue(new Dictionary<string, JSONValue>());
        Assert.ThrowsException<InvalidOperationException>(() => mutation.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_input_is_dict_and_depth_is_1()
    {
        var mutation = new CullMutation
        {
            MaxDepth = new ValueExpression { Value = new JSONValue(1) }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());

        var inputValue = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", new Dictionary<string, JSONValue>
                {
                    { "subkey1", "subvalue1" },
                    { "subkey2", new Dictionary<string, JSONValue>
                        {
                            { "subsubkey1", "subsubvalue1" }
                        }
                    }
                }
            }
        });

        var result = mutation.Evaluate(context, inputValue);

        var expected = new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", new Dictionary<string, JSONValue>() }
        };

        CollectionAssert.AreEquivalent(expected, result.DictValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_dict_and_depth_is_2()
    {
        var mutation = new CullMutation
        {
            MaxDepth = new ValueExpression { Value = new JSONValue(2) }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());

        var inputValue = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", new Dictionary<string, JSONValue>
                {
                    { "subkey1", "subvalue1" },
                    { "subkey2", new Dictionary<string, JSONValue>
                        {
                            { "subsubkey1", "subsubvalue1" }
                        }
                    }
                }
            }
        });

        var result = mutation.Evaluate(context, inputValue);

        var expected = new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", new Dictionary<string, JSONValue>
                {
                    { "subkey1", "subvalue1" },
                    { "subkey2", new Dictionary<string, JSONValue>() }
                }
            }
        };

        CollectionAssert.AreEquivalent(expected, result.DictValue);
    }

    [TestMethod]
    public void test_evaluate_when_dict_is_contained_in_list()
    {
        var mutation = new CullMutation
        {
            MaxDepth = new ValueExpression { Value = new JSONValue(2) }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());

        var inputValue = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", new List<JSONValue>
                {
                    new Dictionary<string, JSONValue>
                    {
                        { "subkey1", "subvalue1" },
                        { "subkey2", new Dictionary<string, JSONValue>
                            {
                                { "subsubkey1", "subsubvalue1" }
                            }
                        }
                    },
                    "listvalue2"
                }
            }
        });

        var result = mutation.Evaluate(context, inputValue);
        var expected = new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", new List<JSONValue>
                {
                    new Dictionary<string, JSONValue>
                    {
                        { "subkey1", "subvalue1" },
                        { "subkey2", new Dictionary<string, JSONValue>() }
                    },
                    "listvalue2"
                }
            }
        };

        CollectionAssert.AreEquivalent(expected, result.DictValue);
    }

    [TestMethod]
    public void test_evaluate_when_dict_in_list_of_lists()
    {
        var mutation = new CullMutation
        {
            MaxDepth = new ValueExpression { Value = new JSONValue(2) }
        };

        var context = new ExpressionContext(new Queuebal.Json.Data.VariableProvider());

        var inputValue = new JSONValue(new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", new List<JSONValue>
                {
                    new Dictionary<string, JSONValue>
                    {
                        { "subkey1", "subvalue1" },
                        { "subkey2", new List<JSONValue>
                            {
                                new Dictionary<string, JSONValue>
                                {
                                    { "subsubkey1", "subsubvalue1" }
                                },
                                "listvalue2"
                            }
                        }
                    },
                }
            }
        });

        var result = mutation.Evaluate(context, inputValue);
        var expected = new Dictionary<string, JSONValue>
        {
            { "key1", "value1" },
            { "key2", new List<JSONValue>
                {
                    new Dictionary<string, JSONValue>
                    {
                        { "subkey1", "subvalue1" },
                        { "subkey2", new List<JSONValue>() },
                    },
                }
            }
        };

        CollectionAssert.AreEquivalent(expected, result.DictValue);
    }
}
