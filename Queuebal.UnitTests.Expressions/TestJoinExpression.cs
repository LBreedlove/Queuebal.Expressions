using Queuebal.Expressions;
using Queuebal.Json;


namespace Queuebal.UnitTests.Expressions;

[TestClass]
public class TestJoinExpression
{
    private JSONValue GetLeftList()
    {
        return new JSONValue(new List<JSONValue>
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
            }),
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "id", new JSONValue(3) },
                { "name", new JSONValue("Jim") }
            }),
        });
    }

    private JSONValue GetRightList()
    {
        return new JSONValue(new List<JSONValue>
        {
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "userId", new JSONValue(1) },
                { "email", new JSONValue("alice@example.com") },
            }),
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "userId", new JSONValue(3) },
                { "email", new JSONValue("jim@example.com") },
            }),
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "userId", new JSONValue(4) },
                { "email", new JSONValue("missing@example.com") },
            }),
        });
    }

    [TestMethod]
    public void test_evaluate_when_lvalues_is_not_list()
    {
        // Arrange
        var expression = new JoinExpression
        {
            LTable = new ValueExpression { Value = 123 },
            RTable = new ValueExpression { Value = GetRightList() },
            LeftKeySelector = new DataSelectorExpression { Path = "id" },
            RightKeySelector = new DataSelectorExpression { Path = "userId" },
            LTableFields = new List<JoinField>(),
            RTableFields = new List<JoinField>()
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_rvalues_is_not_list()
    {
        // Arrange
        var expression = new JoinExpression
        {
            LTable = new ValueExpression { Value = GetLeftList() },
            RTable = new ValueExpression { Value = 123 },
            LeftKeySelector = new DataSelectorExpression { Path = "id" },
            RightKeySelector = new DataSelectorExpression { Path = "userId" },
            LTableFields = new List<JoinField>(),
            RTableFields = new List<JoinField>()
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_lrecord_is_not_dict()
    {
        // Arrange
        var expression = new JoinExpression
        {
            LTable = new ValueExpression { Value = new List<JSONValue> { 123 } },
            RTable = new ValueExpression { Value = GetRightList() },
            LeftKeySelector = new DataSelectorExpression { Path = "id" },
            RightKeySelector = new DataSelectorExpression { Path = "userId" },
            LTableFields = new List<JoinField>(),
            RTableFields = new List<JoinField>()
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_rrecord_is_not_dict()
    {
        // Arrange
        var expression = new JoinExpression
        {
            LTable = new ValueExpression { Value = GetLeftList() },
            RTable = new ValueExpression { Value = new List<JSONValue> { 123 } },
            LeftKeySelector = new DataSelectorExpression { Path = "id" },
            RightKeySelector = new DataSelectorExpression { Path = "userId" },
            LTableFields = new List<JoinField>(),
            RTableFields = new List<JoinField>()
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_lkey_is_list_or_dict()
    {
        // Arrange
        var expression = new JoinExpression
        {
            LTable = new ValueExpression { Value = new List<JSONValue> { new JSONValue(new Dictionary<string, JSONValue> { { "id", new JSONValue(1) } }) } },
            RTable = new ValueExpression { Value = new List<JSONValue>() },
            LeftKeySelector = new ValueExpression { Value = new List<JSONValue> { 1 } },
            RightKeySelector = new DataSelectorExpression { Path = "userId" },
            LTableFields = new List<JoinField>(),
            RTableFields = new List<JoinField>()
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_rkey_is_list_or_dict()
    {
        // Arrange
        var expression = new JoinExpression
        {
            LTable = new ValueExpression { Value = GetLeftList() },
            RTable = new ValueExpression { Value = new List<JSONValue> { new JSONValue(new Dictionary<string, JSONValue> { { "id", new JSONValue(1) } }) } },
            LeftKeySelector = new DataSelectorExpression { Path = "id" },
            RightKeySelector = new ValueExpression { Value = new List<JSONValue> { 1 } },
            LTableFields = new List<JoinField>(),
            RTableFields = new List<JoinField>()
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => expression.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue()));
    }

    [TestMethod]
    public void test_evaluate_when_joining_two_lists_returns_joined_list()
    {
        // Arrange
        var leftList = GetLeftList();
        var rightList = GetRightList();

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
            }),
           new JSONValue(new Dictionary<string, JSONValue>
            {
                { "id", new JSONValue(3) },
                { "fullName", new JSONValue("Jim") },
                { "emailAddress", new JSONValue("jim@example.com") }
            })
         };

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_evaluate_when_field_list_is_star()
    {
        // Arrange
        var leftList = GetLeftList();
        var rightList = GetRightList();

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
                new JoinField { FieldName = "*" }
            }
        }.Evaluate(new ExpressionContext(new Json.Data.DataProvider()), new JSONValue("not used"));

        Assert.IsNotNull(result);
        var expected = new List<JSONValue>
        {
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "id", new JSONValue(1) },
                { "fullName", new JSONValue("Alice") },
                { "userId", new JSONValue(1) },
                { "email", new JSONValue("alice@example.com") }
            }),
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "id", new JSONValue(3) },
                { "fullName", new JSONValue("Jim") },
                { "userId", new JSONValue(3) },
                { "email", new JSONValue("jim@example.com") }
            })
         };

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void test_evaluate_when_field_not_found_uses_null()
    {
        // Arrange
        var leftList = GetLeftList();
        var rightList = GetRightList();

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
                new JoinField { FieldName = "userIdentifier" },
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
                { "userIdentifier", new JSONValue() },
                { "emailAddress", new JSONValue("alice@example.com") }
            }),
            new JSONValue(new Dictionary<string, JSONValue>
            {
                { "id", new JSONValue(3) },
                { "fullName", new JSONValue("Jim") },
                { "userIdentifier", new JSONValue() },
                { "emailAddress", new JSONValue("jim@example.com") }
            })
         };

        Assert.AreEqual(expected, result);
    }
}