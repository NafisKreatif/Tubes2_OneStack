namespace DOMTreeTraversal.Models;

using System.Net;

public enum TokenType
{
    TagOpen,
    TagClose,
    Text
}

public class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public bool IsSelfClosing { get; set; }

    public Token()
    {
    }

    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }
}

public class Tokenizer
{
    private static readonly HashSet<string> VoidTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "area", "base", "br", "col", "embed", "hr", "img", "input",
        "link", "meta", "param", "source", "track", "wbr"
    };

    public List<Token> Tokenize(string html)
    {
        List<Token> tokens = new List<Token>();

        if (string.IsNullOrEmpty(html))
        {
            return tokens;
        }

        int index = 0;
        while (index < html.Length)
        {
            if (html[index] != '<')
            {
                int textEnd = html.IndexOf('<', index);
                if (textEnd < 0)
                {
                    textEnd = html.Length;
                }

                AddTextToken(tokens, html.Substring(index, textEnd - index));
                index = textEnd;
                continue;
            }

            if (StartsWith(html, index, "<!--"))
            {
                int commentEnd = html.IndexOf("-->", index + 4, StringComparison.Ordinal);
                index = commentEnd >= 0 ? commentEnd + 3 : html.Length;
                continue;
            }

            if (StartsWith(html, index, "<!"))
            {
                int declarationEnd = FindTagEnd(html, index + 2);
                index = declarationEnd >= 0 ? declarationEnd + 1 : html.Length;
                continue;
            }

            if (StartsWith(html, index, "<?"))
            {
                int processingEnd = html.IndexOf("?>", index + 2, StringComparison.Ordinal);
                index = processingEnd >= 0 ? processingEnd + 2 : html.Length;
                continue;
            }

            int tagEnd = FindTagEnd(html, index + 1);
            if (tagEnd < 0)
            {
                AddTextToken(tokens, html[index..]);
                break;
            }

            string rawTagContent = html.Substring(index + 1, tagEnd - index - 1).Trim();
            index = tagEnd + 1;

            if (string.IsNullOrEmpty(rawTagContent))
            {
                continue;
            }

            if (rawTagContent[0] == '/')
            {
                string closingTagName = ExtractTagName(rawTagContent[1..]);
                if (!string.IsNullOrEmpty(closingTagName))
                {
                    tokens.Add(new Token(TokenType.TagClose, closingTagName));
                }

                continue;
            }

            bool isSelfClosing = rawTagContent.EndsWith("/", StringComparison.Ordinal);
            if (isSelfClosing)
            {
                rawTagContent = rawTagContent[..^1].TrimEnd();
            }

            string tagName = ExtractTagName(rawTagContent);
            if (string.IsNullOrEmpty(tagName))
            {
                continue;
            }

            Dictionary<string, string> attributes = ParseAttributes(rawTagContent[tagName.Length..]);
            Token openToken = new()
            {
                Type = TokenType.TagOpen,
                Value = tagName,
                Attributes = attributes,
                IsSelfClosing = isSelfClosing || VoidTags.Contains(tagName)
            };
            tokens.Add(openToken);

            if (!openToken.IsSelfClosing && IsRawTextElement(tagName))
            {
                int closingTagStart = FindClosingTagStart(html, index, tagName);
                if (closingTagStart < 0)
                {
                    AddTextToken(tokens, html[index..], preserveWhitespace: true);
                    tokens.Add(new Token(TokenType.TagClose, tagName));
                    break;
                }

                AddTextToken(tokens, html.Substring(index, closingTagStart - index), preserveWhitespace: true);

                int closingTagEnd = FindTagEnd(html, closingTagStart + 2);
                if (closingTagEnd < 0)
                {
                    tokens.Add(new Token(TokenType.TagClose, tagName));
                    break;
                }

                string closingTagName = ExtractTagName(html.Substring(closingTagStart + 2, closingTagEnd - closingTagStart - 2));
                if (!string.IsNullOrEmpty(closingTagName))
                {
                    tokens.Add(new Token(TokenType.TagClose, closingTagName));
                }

                index = closingTagEnd + 1;
            }
        }

        return tokens;
    }

    private static void AddTextToken(List<Token> tokens, string rawText, bool preserveWhitespace = false)
    {
        if (string.IsNullOrEmpty(rawText)) return;

        if (!preserveWhitespace && string.IsNullOrWhiteSpace(rawText)) return;

        string decodedText = preserveWhitespace ? rawText : WebUtility.HtmlDecode(rawText);
        if (string.IsNullOrEmpty(decodedText)) return;

        tokens.Add(new Token(TokenType.Text, decodedText));
    }
    private static int FindTagEnd(string html, int startIndex)
    {
        bool inQuotes = false;
        char quoteCharacter = '\0';

        for (int index = startIndex; index < html.Length; index++)
        {
            char current = html[index];

            if ((current == '"' || current == '\'') && (!inQuotes || current == quoteCharacter))
            {
                if (inQuotes && current == quoteCharacter)
                {
                    inQuotes = false;
                    quoteCharacter = '\0';
                }
                else if (!inQuotes)
                {
                    inQuotes = true;
                    quoteCharacter = current;
                }

                continue;
            }

            if (!inQuotes && current == '>')
            {
                return index;
            }
        }

        return -1;
    }

    private static string ExtractTagName(string tagContent)
    {
        string trimmed = tagContent.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return string.Empty;
        }

        int endIndex = 0;
        while (endIndex < trimmed.Length && !char.IsWhiteSpace(trimmed[endIndex]) && trimmed[endIndex] != '/')
        {
            endIndex++;
        }

        return trimmed[..endIndex].ToLowerInvariant();
    }

    private static Dictionary<string, string> ParseAttributes(string rawAttributes)
    {
        Dictionary<string, string> attributes = new(StringComparer.OrdinalIgnoreCase);
        int index = 0;

        while (index < rawAttributes.Length)
        {
            while (index < rawAttributes.Length && char.IsWhiteSpace(rawAttributes[index]))
            {
                index++;
            }

            if (index >= rawAttributes.Length || rawAttributes[index] == '/')
            {
                break;
            }

            int nameStart = index;
            while (index < rawAttributes.Length &&
                   !char.IsWhiteSpace(rawAttributes[index]) &&
                   rawAttributes[index] != '=' &&
                   rawAttributes[index] != '/')
            {
                index++;
            }

            string attributeName = rawAttributes[nameStart..index];
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                break;
            }

            while (index < rawAttributes.Length && char.IsWhiteSpace(rawAttributes[index]))
            {
                index++;
            }

            string attributeValue = "true";
            if (index < rawAttributes.Length && rawAttributes[index] == '=')
            {
                index++;

                while (index < rawAttributes.Length && char.IsWhiteSpace(rawAttributes[index]))
                {
                    index++;
                }

                if (index < rawAttributes.Length && (rawAttributes[index] == '"' || rawAttributes[index] == '\''))
                {
                    char quote = rawAttributes[index];
                    index++;
                    int valueStart = index;

                    while (index < rawAttributes.Length && rawAttributes[index] != quote)
                    {
                        index++;
                    }

                    attributeValue = rawAttributes[valueStart..Math.Min(index, rawAttributes.Length)];
                    if (index < rawAttributes.Length && rawAttributes[index] == quote)
                    {
                        index++;
                    }
                }
                else
                {
                    int valueStart = index;
                    while (index < rawAttributes.Length &&
                        !char.IsWhiteSpace(rawAttributes[index]))
                    {
                        index++;
                    }

                    attributeValue = rawAttributes[valueStart..index];
                }
            }

            attributes[attributeName.ToLowerInvariant()] = WebUtility.HtmlDecode(attributeValue);
        }

        return attributes;
    }

    private static bool StartsWith(string source, int startIndex, string value)
    {
        return source.AsSpan(startIndex).StartsWith(value, StringComparison.Ordinal);
    }

    private static bool IsRawTextElement(string tagName)
    {
        return tagName.Equals("script", StringComparison.OrdinalIgnoreCase) ||
               tagName.Equals("style", StringComparison.OrdinalIgnoreCase);
    }

    private static int FindClosingTagStart(string html, int startIndex, string tagName)
    {
        string closingTag = $"</{tagName}";
        int searchIndex = startIndex;

        while (searchIndex < html.Length)
        {
            int candidate = html.IndexOf(closingTag, searchIndex, StringComparison.OrdinalIgnoreCase);
            if (candidate < 0)
            {
                return -1;
            }

            int boundaryIndex = candidate + closingTag.Length;
            if (boundaryIndex >= html.Length || char.IsWhiteSpace(html[boundaryIndex]) || html[boundaryIndex] == '>')
            {
                return candidate;
            }

            searchIndex = candidate + 1;
        }

        return -1;
    }
}
