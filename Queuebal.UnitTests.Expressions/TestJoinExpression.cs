using Queuebal.Expressions;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestJoinExpression
{
    [TestMethod]
    public void test_evaluate_when_joining_two_lists_returns_joined_list()
    {
        // Arrange
        var leftList = new JSONValue(new List<JSONValue>
        {
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "id", new JSONValue(1) },
                { "name", new JSONValue("Alice") }
            }),
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "id", new JSONValue(2) },
                { "name", new JSONValue("Bob") }
            })
        });

        var rightList = new JSONValue(new List<JSONValue>
        {
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "userId", new JSONValue(1) },
                { "email", new JSONValue("alice@example.com") },
            }),
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "userId", new JSONValue(3) },
                { "email", new JSONValue("missing@example.com") },
            })
        });

        var result = new JoinExpression
        {
            LTable = new ValueExpression { Value = leftList },
            RTable = new ValueExpression { Value = rightList },
            LeftKeySelector = new DataSelectorExpression { Path = "id" },
            RightKeySelector = new DataSelectorExpression { Path = "userId" },
            LTableFields = new List<JoinField>
            {
                new JoinField { FieldName = "id" },
                new JoinField { FieldName = "name", Alias = "fullName" }
            },
            RTableFields = new List<JoinField>
            {
                new JoinField { FieldName = "email", Alias = "emailAddress" }
            }
        }.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue("not used"));

        Assert.IsNotNull(result);
        var expected = new List<JSONValue>
        {
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "id", new JSONValue(1) },
                { "fullName", new JSONValue("Alice") },
                { "emailAddress", new JSONValue("alice@example.com") }
            })
        };

        Assert.AreEqual(expected, result);
    }
}