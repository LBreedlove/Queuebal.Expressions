using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.UnitTests.Json.Data;


[TestClass]
public class TestTokenizer
{
    [TestMethod]
    public void test_get_tokens_when_unmatched_opening_bracket_throws_exception()
    {
        var text = "this is a test ${ with an extra ${ opening bracket";
        var variableProvider = new VariableProvider();
        Assert.ThrowsException<FormatException>(() => Tokenizer.GetTokens(text, variableProvider).ToList());
    }

    [TestMethod]
    public void test_get_tokens_when_unmatched_closing_bracket_throws_exception()
    {
        var text = "this is a ${test} with} an extra closing bracket";
        var variableProvider = new VariableProvider();
        variableProvider.AddValue("test", new JSONValue("value"));
        Assert.ThrowsException<FormatException>(() => Tokenizer.GetTokens(text, variableProvider).ToList());
    }

    [TestMethod]
    public void test_get_tokens_when_opening_bracket_is_escaped_is_not_replaced()
    {
        var text = "this is a test \\${ with an escaped opening bracket";
        var variableProvider = new VariableProvider();
        var tokens = Tokenizer.GetTokens(text, variableProvider).ToList();

        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual("this is a test ${ with an escaped opening bracket", tokens[0].Text);
        Assert.IsFalse(tokens[0].IsPlaceholder);
    }

    [TestMethod]
    public void test_get_tokens_when_text_contains_unclosed_placeholder_throws_exception()
    {
        var text = "this is a test ${placeholder with no closing bracket";
        var variableProvider = new VariableProvider();
        Assert.ThrowsException<FormatException>(() => Tokenizer.GetTokens(text, variableProvider).ToList());
    }

    [TestMethod]
    public void test_get_tokens_when_text_contains_escaped_extra_closing_paren_is_ignored()
    {
        var text = "this is a test ${placeholder} with}} an escaped closing bracket";
        var variableProvider = new VariableProvider();
        variableProvider.AddValue("placeholder", new JSONValue("value"));

        var tokens = Tokenizer.GetTokens(text, variableProvider).ToList();

        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual("this is a test ", tokens[0].Text);
        Assert.IsFalse(tokens[0].IsPlaceholder);

        Assert.AreEqual("placeholder", tokens[1].Text);
        Assert.IsTrue(tokens[1].IsPlaceholder);
        Assert.AreEqual("value", tokens[1].ReplacementValue?.StringValue);

        Assert.AreEqual(" with} an escaped closing bracket", tokens[2].Text);
        Assert.IsFalse(tokens[2].IsPlaceholder);
    }

    [TestMethod]
    public void test_get_tokens_when_closing_bracket_in_placeholder_is_escaped()
    {
        var text = "this is a test ${placeholder}} with} an escaped closing bracket";
        var variableProvider = new VariableProvider();
        variableProvider.AddValue("placeholder} with", new JSONValue("value"));

        var tokens = Tokenizer.GetTokens(text, variableProvider).ToList();

        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual("this is a test ", tokens[0].Text);
        Assert.IsFalse(tokens[0].IsPlaceholder);

        Assert.AreEqual("placeholder} with", tokens[1].Text);
        Assert.IsTrue(tokens[1].IsPlaceholder);
        Assert.AreEqual("value", tokens[1].ReplacementValue?.StringValue);

        Assert.AreEqual(" an escaped closing bracket", tokens[2].Text);
        Assert.IsFalse(tokens[2].IsPlaceholder);
    }

    [TestMethod]
    public void test_get_tokens_when_closing_bracket_found_after_closed_placeholder_token()
    {
        var text = "this is a ${test}} with an extra closing bracket";
        var variableProvider = new VariableProvider();
        variableProvider.AddValue("test", new JSONValue("value"));

        Assert.ThrowsExactly<FormatException>(() => Tokenizer.GetTokens(text, variableProvider).ToList());
    }

    [TestMethod]
    public void test_get_tokens_when_replacement_value_is_not_found()
    {
        var text = "this is a test ${placeholder} with no replacement value";
        var variableProvider = new VariableProvider();

        Assert.ThrowsException<KeyNotFoundException>(() => Tokenizer.GetTokens(text, variableProvider).ToList());
    }

    [TestMethod]
    public void test_get_tokens_when_text_contains_tokens_within_brackets()
    {
        var text = "this is a test ${placeholder1} with ${placeholder2} tokens";
        var variableProvider = new VariableProvider();
        var variableProviderValues = new Dictionary<string, JSONValue>
        {
            { "placeholder1", new JSONValue("value1") },
            { "placeholder2", new JSONValue("value2") }
        };

        using (variableProvider.WithScope("testScope", variableProviderValues))
        {
            var tokens = Tokenizer.GetTokens(text, variableProvider).ToList();

            Assert.AreEqual(5, tokens.Count);

            Assert.AreEqual("this is a test ", tokens[0].Text);
            Assert.IsFalse(tokens[0].IsPlaceholder);

            Assert.AreEqual("placeholder1", tokens[1].Text);
            Assert.IsTrue(tokens[1].IsPlaceholder);
            Assert.AreEqual("value1", tokens[1].ReplacementValue?.StringValue);

            Assert.AreEqual(" with ", tokens[2].Text);
            Assert.IsFalse(tokens[2].IsPlaceholder);

            Assert.AreEqual("placeholder2", tokens[3].Text);
            Assert.IsTrue(tokens[3].IsPlaceholder);
            Assert.AreEqual("value2", tokens[3].ReplacementValue?.StringValue);

            Assert.AreEqual(" tokens", tokens[4].Text);
            Assert.IsFalse(tokens[4].IsPlaceholder);
        }
    }

    [TestMethod]
    public void test_evaluate_when_input_value_is_empty_returns_input()
    {
        var variableProvider = new VariableProvider();
        var result = Tokenizer.Evaluate("", variableProvider);
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void test_evaluate_when_input_is_single_variable_returns_variable_value()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddValue("test", new JSONValue(3.14f));

        var result = Tokenizer.Evaluate("${test}", variableProvider);
        Assert.AreEqual(3.14f, result.FloatValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_does_not_contain_placeholder_returns_input_as_string()
    {
        var variableProvider = new VariableProvider();
        var inputValue = "this is a test string";
        var result = Tokenizer.Evaluate(inputValue, variableProvider);
        Assert.AreEqual(inputValue, result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_input_contains_multiple_tokens_returns_concatenated_string()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddValue("test1", new JSONValue("Hello"));
        variableProvider.AddValue("test2", new JSONValue(123));
        variableProvider.AddValue("test3", new JSONValue("World"));

        var inputValue = "${test1} ${test2} ${test3}!";
        var result = Tokenizer.Evaluate(inputValue, variableProvider);
        Assert.AreEqual("Hello 123 World!", result.StringValue);
    }

    [TestMethod]
    public void test_evaluate_when_stored_replacement_value_is_null()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddValue("test", new JSONValue());

        var inputValue = "${test}";
        var result = Tokenizer.Evaluate(inputValue, variableProvider);
        Assert.IsTrue(result.IsNull);
    }
}
