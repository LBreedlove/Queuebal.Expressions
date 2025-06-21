using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestDynamicDictExpression
{
    [TestMethod]
    public void test_evaluate_when_key_is_not_string_throws_exception()
    {
        var expression = new DynamicDictExpression
        {
            Entries = new List<DynamicDictEntry>
            {
                new DynamicDictEntry
                {
                    Key   = new ValueExpression { Value = new JSONValue(123) }, // Non-string key
                    Value = new ValueExpression { Value = new JSONValue("value") }
                }
            }
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());
        var inputValue = new JSONValue("input");
        Assert.ThrowsException<InvalidOperationException>(() => expression.Evaluate(context, inputValue));
    }

    [TestMethod]
    public void test_evaluate_when_entry_condition_is_false_excludes_entry()
    {
        var expression = new DynamicDictExpression
        {
            Entries = new List<DynamicDictEntry>
            {
                new DynamicDictEntry
                {
                    Key   = new ValueExpression { Value = new JSONValue("key1") },
                    Value = new ValueExpression { Value = new JSONValue("value1") },
                    Condition = new ConditionExpression{
                        Condition = new ConditionSet
                        {
                            Conditions = new List<ICondition>
                            {
                                new EqualsCondition
                                {
                                    // This condition will evaluate to false
                                    ComparerValue = new ValueExpression { Value = new JSONValue("value3") }
                                }
                            }
                        }
                    }
                },
                new DynamicDictEntry
                {
                    Key   = new ValueExpression { Value = new JSONValue("key2") },
                    Value = new ValueExpression { Value = new JSONValue("value2") }
                }
            }
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());
        var inputValue = new List<JSONValue> { new JSONValue("input") };
        var result = expression.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsDict);
        Assert.AreEqual(1, result.DictValue.Count);

        Assert.IsTrue(result.DictValue.ContainsKey("key2"));
        Assert.AreEqual("value2", result.DictValue["key2"].StringValue);

        // The excluded entry should not be present
        Assert.IsFalse(result.DictValue.ContainsKey("key1"));
    }

    [TestMethod]
    public void test_evaluate_when_entry_condition_is_true_includes_entry()
    {
        var expression = new DynamicDictExpression
        {
            Entries = new List<DynamicDictEntry>
            {
                new DynamicDictEntry
                {
                    Key   = new ValueExpression { Value = new JSONValue("key1") },
                    Value = new ValueExpression { Value = new JSONValue("value1") },
                    Condition = new ConditionExpression
                    {
                        Condition = ConditionSet.AlwaysTrue
                    }
                },
                new DynamicDictEntry
                {
                    Key   = new ValueExpression { Value = new JSONValue("key2") },
                    Value = new ValueExpression { Value = new JSONValue("value2") }
                }
            }
        };
        var context = new ExpressionContext(new Json.Data.DataProvider());
        var inputValue = new List<JSONValue> { new JSONValue("input") };
        var result = expression.Evaluate(context, inputValue);
        Assert.IsTrue(result.IsDict);
        Assert.AreEqual(2, result.DictValue.Count);
        Assert.IsTrue(result.DictValue.ContainsKey("key1"));
        Assert.AreEqual("value1", result.DictValue["key1"].StringValue);
        Assert.IsTrue(result.DictValue.ContainsKey("key2"));
        Assert.AreEqual("value2", result.DictValue["key2"].StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_no_condition_is_present_includes_all_entries()
    {
        var expression = new DynamicDictExpression
        {
            Entries = new List<DynamicDictEntry>
            {
                new DynamicDictEntry
                {
                    Key   = new ValueExpression { Value = new JSONValue("key1") },
                    Value = new ValueExpression { Value = new JSONValue("value1") }
                },
                new DynamicDictEntry
                {
                    Key   = new ValueExpression { Value = new JSONValue("key2") },
                    Value = new ValueExpression { Value = new JSONValue("value2") }
                }
            }
        };

        var context = new ExpressionContext(new Json.Data.DataProvider());
        var inputValue = new List<JSONValue> { new JSONValue("input") };
        var result = expression.Evaluate(context, inputValue);

        Assert.IsTrue(result.IsDict);
        Assert.AreEqual(2, result.DictValue.Count);

        Assert.IsTrue(result.DictValue.ContainsKey("key1"));
        Assert.AreEqual("value1", result.DictValue["key1"].StringValue);

        Assert.IsTrue(result.DictValue.ContainsKey("key2"));
        Assert.AreEqual("value2", result.DictValue["key2"].StringValue);
    }
}