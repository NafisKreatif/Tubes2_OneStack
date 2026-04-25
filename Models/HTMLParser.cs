namespace DOMTreeTraversal.Models;

public class HTMLParser
{
    public static async Task<string> Scrape(string url)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(15);
        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");

        string normalizedUrl = NormalizeUrl(url);
        bool inputHasExplicitScheme = HasExplicitHttpScheme(url);
        try
        {
            return await client.GetStringAsync(normalizedUrl);
        }
        catch (HttpRequestException ex) when (!inputHasExplicitScheme &&
                                             normalizedUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
                                             ex.StatusCode is null)
        {
            // Retry dengan http:// jika https gagal
            string httpUrl = "http://" + normalizedUrl["https://".Length..];
            return await client.GetStringAsync(httpUrl);
        }
    }

    public static DOMTree ParseFromString(string html)
    {
        DOMTree domTree = new DOMTree();
        Tokenizer tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize(html);

        Stack<DOMNode> stack = new Stack<DOMNode>();
        stack.Push(domTree.Root);
        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.TagOpen:
                    {
                        DOMNode node = domTree.AddChild(stack.Peek(), token.Value);
                        ApplyAttributes(node, token);
                        if (!token.IsSelfClosing)
                        {
                            stack.Push(node);
                        }
                        break;
                    }
                case TokenType.Text:
                    {
                        domTree.AddChild(stack.Peek(), "", token.Value);
                        break;
                    }
                case TokenType.TagClose:
                    {
                        CloseNode(stack, token.Value);
                        break;
                    }
                default:
                    break;
            }
        }

        return domTree;
    }

    public static async Task<DOMTree> ParseFromUrl(string url)
    {
        string html = await Scrape(url);
        return ParseFromString(html);
    }

    private static void ApplyAttributes(DOMNode node, Token token)
    {
        node.Attribute = new Dictionary<string, string>(token.Attributes, StringComparer.OrdinalIgnoreCase);

        if (token.Attributes.TryGetValue("id", out string? id))
        {
            node.Id = id;
        }

        if (token.Attributes.TryGetValue("class", out string? classes))
        {
            node.Class = classes
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }
    }

    private static void CloseNode(Stack<DOMNode> stack, string closingTag)
    {
    bool found = stack.Any(n => 
        string.Equals(n.Tag, closingTag, StringComparison.OrdinalIgnoreCase));
    
    if (!found) return;

    while (stack.Count > 1 &&
           !string.Equals(stack.Peek().Tag, closingTag, StringComparison.OrdinalIgnoreCase))
    {
        stack.Pop();
    }

    if (stack.Count > 1)
        stack.Pop();
    }

    private static string NormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL tidak boleh kosong.", nameof(url));

        string trimmed = url.Trim();

        if (Uri.TryCreate(trimmed, UriKind.Absolute, out Uri? absoluteUri)
            && (absoluteUri.Scheme == "https" || absoluteUri.Scheme == "http"))
        {
            if (string.IsNullOrWhiteSpace(absoluteUri.Host))
                throw new UriFormatException("Host URL tidak valid.");

            return absoluteUri.ToString();
        }

        if (Uri.TryCreate(trimmed, UriKind.Absolute, out absoluteUri))
            throw new UriFormatException("URL harus menggunakan http:// atau https://.");

        if (trimmed.Contains("://", StringComparison.Ordinal))
            throw new UriFormatException("Format URL tidak valid.");

        if (Uri.TryCreate($"https://{trimmed}", UriKind.Absolute, out absoluteUri) &&
            !string.IsNullOrWhiteSpace(absoluteUri.Host))
            return absoluteUri.ToString();

        throw new UriFormatException("Format URL tidak valid.");
    }

    private static bool HasExplicitHttpScheme(string url)
    {
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out Uri? absoluteUri))
            return false;

        return absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps;
    }
}
