using System.Text;
using System.Text.RegularExpressions;
using Queuebal.Json;

namespace Queuebal.Json.Data;


public class Token
{
    private readonly string _text;
    private readonly JSONValue _replacementValue;

    public Token(string text)
    {
        _text = text;
        _replacementValue = new();
        IsPlaceholder = false;
    }

    public Token(string text, JSONValue replacementValue)
    {
        _text = text;
        _replacementValue = replacementValue;
        IsPlaceholder = true;
    }

    public string Text => _text;
    public bool IsPlaceholder { get; }
    public JSONValue ReplacementValue =>
        IsPlaceholder ? _replacementValue : throw new InvalidOperationException("This token is not a placeholder.");
}


public static class VariableReplacement
{
    /// <summary>
    /// Evaluates the input value by replacing any placeholders with their corresponding values
    /// from the provided data provider. If the input value does not contain any placeholders,
    /// it returns the input value as is.
    /// If a placeholder is found but has no replacement value, a KeyNotFoundException is thrown.
    /// If the input value represents a single token (i.e. a direct variable or just text),
    /// it returns the variable/text value directly.
    /// If the input value results in multiple tokens, it concatenates their string representations
    /// and returns the concatenated string as a JSONValue.
    /// </summary>
    /// <param name="inputValue">The input string to tokenize and get the replacement value(s) for.</param>
    /// <param name="variableProvider">The VariableProvider to use to find the replacement values.</param>
    /// <returns>A JSONValue representing the result of the evaluation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when a placeholder token is found, but its value
    /// was not found in the VariableProvider.
    /// </exception>
    public static JSONValue Evaluate(string inputValue, VariableProvider variableProvider)
    {
        var tokens = GetTokens(inputValue, variableProvider);
        if (!tokens.Any())
        {
            return inputValue;
        }

        // if a single token was returned (i.e. the input value represented a direct variable or just text),
        // we can return the variable/text value directly.
        if (tokens.Count() == 1)
        {
            var token = tokens.First();
            if (token.IsPlaceholder)
            {
                return token.ReplacementValue!;
            }

            return new JSONValue(token.Text);
        }

        // if the input resulted in multiple tokens, we need to convert them
        // to strings and concatenate them.
        StringBuilder resultBuilder = new StringBuilder();
        foreach (var token in tokens)
        {
            if (token.IsPlaceholder)
            {
                resultBuilder.Append(token.ReplacementValue.ToString());
            }
            else
            {
                resultBuilder.Append(token.Text);
            }
        }

        return resultBuilder.ToString();
    }

    /// <summary>
    /// Gets the tokens from the input value.
    /// </summary>
    /// <param name="inputValue"></param>
    /// <param name="variableProvider"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IEnumerable<Token> GetTokens(string inputValue, VariableProvider variableProvider)
    {
        bool inPlaceholder = false;
        var currentTokenText = new StringBuilder();
        for (int index = 0; index < inputValue.Length; ++index)
        {
            var c = inputValue[index];
            var nextChar = index + 1 < inputValue.Length ? inputValue[index + 1] : '\0';

            if (c == '\\')
            {
                if (nextChar == '$')
                {
                    currentTokenText.Append("$");
                    ++index;
                    continue;
                }
                else if (nextChar == '\\')
                {
                    currentTokenText.Append('\\');
                    ++index;
                    continue;
                }
                else
                {
                    throw new FormatException($"Unrecognized escape sequence: \\{nextChar}");
                }
            }

            if (c == '}' && nextChar == '}')
            {
                currentTokenText.Append(c);
                ++index; // Skip the next '}' since we are treating '}}' as a literal
                continue;
            }

            if (c == '$' && nextChar == '{')
            {
                if (currentTokenText.Length > 0)
                {
                    if (inPlaceholder)
                    {
                        // If the current token starts with '{', it means we have an unclosed placeholder
                        throw new FormatException("Unclosed placeholder found in input.");
                    }

                    // If we have accumulated characters before the '{', yield them as a regular token
                    yield return new Token(currentTokenText.ToString());

                    currentTokenText.Clear();
                }

                inPlaceholder = true;
                currentTokenText.Append(c);
                currentTokenText.Append(nextChar);

                // skip nextChar since we consumed it.
                ++index;
            }
            else if (c == '}')
            {
                if (!inPlaceholder)
                {
                    // If we encounter '}' without a matching '${', it's an error
                    throw new FormatException("Unmatched '}' found in input.");
                }

                // Skip the opening '${' chars, we didn't add the closing '}' to currentTokenText
                var tokenText = currentTokenText.ToString().Substring(2, currentTokenText.Length - 2);
                var replacementValue = variableProvider.GetValue(tokenText);
                if (replacementValue == null)
                {
                    throw new KeyNotFoundException($"Token '{tokenText}' has no replacement value.");
                }

                yield return new Token(tokenText, replacementValue);

                currentTokenText.Clear();
                inPlaceholder = false;
            }
            else
            {
                if (c != '\\')
                {
                    currentTokenText.Append(c);
                }
            }
        }

        // If we reach the end of the input and are still in a placeholder, it means
        // we have an unclosed placeholder.
        if (inPlaceholder)
        {
            throw new FormatException("Unclosed placeholder found in input.");
        }

        // If there are any remaining characters in currentTokenText, yield them as a regular token
        if (currentTokenText.Length > 0)
        {
            yield return new Token(currentTokenText.ToString());
        }
    }
}
