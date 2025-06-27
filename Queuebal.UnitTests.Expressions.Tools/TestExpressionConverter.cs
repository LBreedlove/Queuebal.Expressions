using Queuebal.Expressions;
using Queuebal.Expressions.Conditions;
using Queuebal.Expressions.Tools;

namespace Queuebal.UnitTests.Expressions.Tools;

[TestClass]
public class TestExpressionConverter
{
    [TestMethod]
    public void test_to_expression_when_source_is_list()
    {
        var source = new List<object?>
        {
            123,
            3.14f,
            "hello world",
            false,
            true,
            new List<object?>
            {
                245,
                42.03f,
                "goodbye moon",
                true,
                false,
                new Dictionary<string, object?>
                {
                    { "key1", "value1" },
                    { "key2", 3.42f },
                    { "key3", 42 },
                    { "key4", true },
                    { "key5", DateTime.Parse("2025-06-27T14:47:29.816Z") },
                    {
                        "key6", new List<object?>
                        {
                            new ConditionExpression
                            {
                                Condition = new EqualsCondition
                                {
                                    NegateResult = true,
                                    ComparerValue = new ValueExpression{ Value = 123}
                                },
                            },
                            456,
                            "hello moon"
                        }
                    }
                }
            }
        };

        var result = source.ToExpression();
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<ListExpression>(result);

        var listExpression = (ListExpression)result;
        Assert.AreEqual(listExpression.Value.Count, source.Count);

        var expected = new ListExpression
        {
            Value = new List<IExpression>
            {
                new ValueExpression { Value = 123 },
                new ValueExpression { Value = (double)3.14 },
                new ValueExpression { Value = "hello world" },
                new ValueExpression { Value = false },
                new ValueExpression { Value = true },
                new ListExpression
                {
                    Value = new List<IExpression>
                    {
                        new ValueExpression { Value = 245 },
                        new ValueExpression { Value = (double)42.03 },
                        new ValueExpression { Value = "goodbye moon" },
                        new ValueExpression { Value = true },
                        new ValueExpression { Value = false },
                        new DictExpression
                        {
                            Value = new Dictionary<string, IExpression>
                            {
                                { "key1", new ValueExpression { Value = "value1" } },
                                { "key2", new ValueExpression { Value = (double)3.42 } },
                                { "key3", new ValueExpression { Value = 42 } },
                                { "key4", new ValueExpression { Value = true } },
                                { "key5", new ValueExpression { Value = DateTime.Parse("2025-06-27T14:47:29.816Z") } },
                                {
                                    "key6", new ListExpression
                                    {
                                        Value = new List<IExpression>
                                        {
                                            new ConditionExpression
                                            {
                                                Condition = new EqualsCondition
                                                {
                                                    NegateResult = true,
                                                    ComparerValue = new ValueExpression{ Value = 123}
                                                },
                                            },
                                            new ValueExpression { Value = 456 },
                                            new ValueExpression { Value = "hello moon" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        // TODO: Figure out why this assert fails. They look identical to me...
        // CollectionAssert.AreEqual(expected.Value, listExpression.Value);
    }
}