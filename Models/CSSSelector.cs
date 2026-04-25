#nullable enable
namespace DOMTreeTraversal.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public enum TraversalMethod
{
    DFS,    
    BFS
} 

public static class CSSSelector
{
    public static List<DOMNode> traversalNodes = new List<DOMNode>();
    public static (List<DOMNode>, TimeSpan) QuerySelector(DOMTree tree, string selector, int maxSelected, TraversalMethod method = TraversalMethod.DFS)
    {
        return QuerySelector(tree.Root, selector, maxSelected, method);
    }
    public static (List<DOMNode>, TimeSpan) QuerySelector(DOMNode root, string selector, int maxSelected, TraversalMethod method = TraversalMethod.DFS)
    {
        traversalNodes = new();
        if (string.IsNullOrWhiteSpace(selector)) return (new List<DOMNode>(), new TimeSpan()); 

        var tokens = TokenizeSelector(selector);
        if (tokens.Count == 0) return (new List<DOMNode>(), new TimeSpan());
        
        DateTime startTime = DateTime.Now;

        var currNodes = GetNodes(root, method, n => MatchSelector(n, tokens[0]));
        if (tokens.Count == 1 && currNodes.Count > maxSelected)
        {
            var temp = new List<DOMNode>();
            int i = 0;
            foreach (var nodes in currNodes)
            {
                if (i < maxSelected) temp.Add(nodes);
                else break;
                i++;
            }
            currNodes = temp;
        }
        
        // Filterrrrrrrrr
        int lastToken = tokens.Count - 2;

        for (int i = 1; i < tokens.Count; i += 2) 
        {
            // ganjil janggal?
            if (i + 1 >= tokens.Count) break;

            bool isLastToken = i == lastToken;
            string combinator = tokens[i];
            string nextSelector = tokens[i + 1];
            HashSet<DOMNode> nextNodes = new HashSet<DOMNode>();

            foreach (var node in currNodes)
            {
                if (maxSelected == 0) break;
                traversalNodes.Add(node);
                
                if (combinator == " ") // Descendant 
                {
                    var descendants = GetNodes(node, method, n => MatchSelector(n, nextSelector));
                    foreach (var desc in descendants)
                    {
                        if (maxSelected == 0) break;
                        traversalNodes.Add(desc);
                        if (MatchSelector(desc, nextSelector)) 
                        {
                            if (isLastToken && nextNodes.Add(desc)) maxSelected--;
                        }
                    }
                }
                else if (combinator == ">") // Child
                {
                    foreach (var child in node.Children)
                    {
                        if (maxSelected == 0) break;
                        traversalNodes.Add(child);
                        if (MatchSelector(child, nextSelector))
                        {
                            if (isLastToken && nextNodes.Add(child)) maxSelected--;
                        } 
                    }
                }
                else if (combinator == "+") // Adjacent Sibling
                {
                    var sibling = GetNextElementSibling(node);
                    if (sibling != null) traversalNodes.Add(sibling);
                    if (sibling != null && MatchSelector(sibling, nextSelector))
                    {
                        if (isLastToken && nextNodes.Add(sibling)) maxSelected--;
                    }
                }
                else if (combinator == "~") // General Sibling
                {
                    var siblings = GetNextElementSiblings(node);
                    foreach (var sibling in siblings)
                    {
                        if (maxSelected == 0) break;
                        traversalNodes.Add(sibling);
                        if (MatchSelector(sibling, nextSelector))
                        {
                            if (isLastToken && nextNodes.Add(sibling)) maxSelected--;
                        } 
                    }
                }
            }
            
            currNodes = nextNodes.ToList();
        }

        DateTime endTime = DateTime.Now;
        TimeSpan runTime = endTime - startTime;

        return (currNodes, runTime);
    }

    private static List<DOMNode> GetNodes(DOMNode startNode, TraversalMethod method, Func<DOMNode, bool> condition)
    {
        return method == TraversalMethod.DFS 
            ? GetNodesDFS(startNode, condition) 
            : GetNodesBFS(startNode, condition);
    }

    private static List<DOMNode> GetNodesDFS(DOMNode node, Func<DOMNode, bool> condition)
    {
        var result = new List<DOMNode>();
        TraverseDFS(node, result, condition);
        return result;
    }

    private static void TraverseDFS(DOMNode node, List<DOMNode> result, Func<DOMNode, bool> condition)
    {
        traversalNodes.Add(node);
        if (condition(node)) result.Add(node);
        foreach (var child in node.Children) TraverseDFS(child, result, condition);
    }

    private static List<DOMNode> GetNodesBFS(DOMNode startNode, Func<DOMNode, bool> condition)
    {
        var result = new List<DOMNode>();
        var queue = new Queue<DOMNode>();
        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            traversalNodes.Add(current);
            if (condition(current)) result.Add(current);

            foreach (var child in current.Children) queue.Enqueue(child);
        }

        return result;
    }

    private static bool MatchSelector(DOMNode node, string selector)
    {
        if (node.IsTextNode()) return false; 
        if (selector == "*") return true;    

        string? tag = null;
        string? id = null;
        List<string> classes = new List<string>();
        List<(string attr, string op, string val)> attributes = new List<(string, string, string)>();

        int i = 0;
        while (i < selector.Length)
        {
            if (selector[i] == '#') // ID 
            {
                int start = ++i;
                while (i < selector.Length && selector[i] != '#' && selector[i] != '.' && selector[i] != '[') i++;
                id = selector.Substring(start, i - start);
            }
            else if (selector[i] == '.') // Class
            {
                int start = ++i;
                while (i < selector.Length && selector[i] != '#' && selector[i] != '.' && selector[i] != '[') i++;
                classes.Add(selector.Substring(start, i - start));
            }
            else if (selector[i] == '[') // Attribute
            {
                int start = ++i;
                while (i < selector.Length && selector[i] != ']') i++; // Asumsi gaada closing bracket error
                string attrContent = selector.Substring(start, i - start);

                attributes.Add(ParseAttribute(attrContent));

                if (i < selector.Length && selector[i] == ']') i++;
            }
            else // Tag
            {
                int start = i;
                while (i < selector.Length && selector[i] != '#' && selector[i] != '.' && selector[i] != '[') i++;
                tag = selector.Substring(start, i - start);
            }
        }

        // Bandingin tag selector dan tag node
        if (tag != null && tag != "*" && !string.Equals(node.Tag, tag, StringComparison.OrdinalIgnoreCase)) return false;
        
        // Bandingin id selector dan id node
        if (id != null && !string.Equals(node.Id, id, StringComparison.OrdinalIgnoreCase)) return false;

        // Bandingin class node dan class selector      
        foreach (var c in classes)
        {
            if (!node.Class.Contains(c)) return false;
        }

        // Bandingin attribute node dan attribute selector
        foreach (var a in attributes)
        {
            if (!node.Attribute.TryGetValue(a.attr, out var val)) 
                return false;

            if (a.op == "=" && !string.Equals(val, a.val, StringComparison.OrdinalIgnoreCase)) 
                return false;

            if (a.op == "~=") 
            {
                var words = val.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if(!words.Contains(a.val, StringComparer.OrdinalIgnoreCase)) 
                    return false;
            }

            if (a.op == "|=")
            {
                if(!string.Equals(val, a.val, StringComparison.OrdinalIgnoreCase) && !val.StartsWith(a.val + "-", StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            if (a.op == "^=" && !val.StartsWith(a.val, StringComparison.OrdinalIgnoreCase)) 
                return false;

            if (a.op == "$=" && !val.EndsWith(a.val, StringComparison.OrdinalIgnoreCase)) 
                return false;

            if (a.op == "*=" && !val.Contains(a.val, StringComparison.OrdinalIgnoreCase)) 
                return false;
        }

        return true;
    }

    private static (string, string, string) ParseAttribute(string contents)
    {
        (string, string, string) parsed = new ("", "", "");
        int opIdx = contents.IndexOf("="); 
        string op = "=";

        if (opIdx == -1) parsed = (contents.Trim(), "", "");
        else
        {
            string modifier = "~|^$*";
            int attrEnd = opIdx - 1;
            int valStart = opIdx + 1;

            if(opIdx > 0 && modifier.Contains(contents[attrEnd]))
            {
                op = contents.Substring(attrEnd, 2);
                attrEnd--;
            } 

            string attr = contents.Substring(0, attrEnd + 1).Trim();
            string val  = contents.Substring(valStart).Trim();

            if (val.Length >= 2 && ((val.StartsWith("\"") && val.EndsWith("\"")) || (val.StartsWith("'") && val.EndsWith("'"))))
                val = val.Substring(1, val.Length - 2);

            parsed = (attr, op, val);
        }

        return parsed;
    }

    private static List<string> TokenizeSelector(string selector)
    {
        List<string> tokens = new List<string>();
        bool insideBrackets = false;
        bool insideQuotes = false;
        char quoteChar = '\0';
        
        string currentToken = "";

        for (int i = 0; i < selector.Length; i++)
        {
            char c = selector[i];

            if (insideQuotes)
            {
                currentToken += c;
                if (c == quoteChar) insideQuotes = false;
            }
            else if (insideBrackets)
            {
                currentToken += c;
                if (c == '"' || c == '\'') 
                {
                    insideQuotes = true;
                    quoteChar = c;
                }
                else if (c == ']') insideBrackets = false;
            }
            else
            {
                if (c == '"' || c == '\'')
                {
                    insideQuotes = true;
                    quoteChar = c;
                    currentToken += c;
                }
                else if (c == '[')
                {
                    insideBrackets = true;
                    currentToken += c;
                }
                else if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        tokens.Add(currentToken);
                        currentToken = "";
                    }
                    if (tokens.Count > 0 && tokens.Last() != ">" && tokens.Last() != "+" 
                        && tokens.Last() != "~" && tokens.Last() != " ")
                    {
                        tokens.Add(" ");
                    }
                }
                else if (c == '>' || c == '+' || c == '~')
                {
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        tokens.Add(currentToken);
                        currentToken = "";
                    }
                    
                    if (tokens.Count > 0 && tokens.Last() == " ") 
                        tokens.RemoveAt(tokens.Count - 1);
                    
                    tokens.Add(c.ToString());
                }
                else currentToken += c;
            }
        }

        if (!string.IsNullOrEmpty(currentToken)) tokens.Add(currentToken);

        while (tokens.Count > 0 && tokens.Last() == " ") tokens.RemoveAt(tokens.Count - 1);

        return tokens;
    }

    private static DOMNode? GetNextElementSibling(DOMNode node)
    {
        if (node.Parent == null) return null;
        var siblings = node.Parent.Children;
        int index = siblings.IndexOf(node);
        
        if (index >= 0)
        {
            for (int i = index + 1; i < siblings.Count; i++)
            {
                if (!siblings[i].IsTextNode()) return siblings[i];
            }
        }
        return null;
    }

    private static List<DOMNode> GetNextElementSiblings(DOMNode node)
    {
        var result = new List<DOMNode>();
        if (node.Parent == null) return result;
        
        var siblings = node.Parent.Children;
        int index = siblings.IndexOf(node);
        
        if (index >= 0)
        {
            for (int i = index + 1; i < siblings.Count; i++)
            {
                traversalNodes.Add(siblings[i]);
                if (!siblings[i].IsTextNode()) result.Add(siblings[i]);
            }
        }
        return result;
    }
}