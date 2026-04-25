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
    public static (List<DOMNode>, List<DOMNode>) QuerySelectorAll(DOMTree tree, string selector, TraversalMethod method = TraversalMethod.DFS)
    {
        return QuerySelectorAll(tree.Root, selector, method);
    }
    public static (List<DOMNode>, List<DOMNode>) QuerySelectorAll(DOMNode root, string selector, TraversalMethod method = TraversalMethod.DFS)
    {
        // Empty Query
        if (string.IsNullOrWhiteSpace(selector)) return (new List<DOMNode>(), new List<DOMNode>()); 

        var tokens = TokenizeSelector(selector);
        // no valid Query Tokenzzz
        if (tokens.Count == 0) return (new List<DOMNode>(), new List<DOMNode>());

        var allNodes = GetAllNodes(root, method);
        // yang memenuhi Query Token pertama dari urutan traversal
        var currentNodes = allNodes.Where(n => MatchesSimpleSelector(n, tokens[0])).ToList(); 
        
        List<DOMNode> traversalNodes = new List<DOMNode>(); // jujur ini bakal gede banget, kalo mw sebenernya bisa di allNodes but like yh

        // Filterrrrrrrrr
        for (int i = 1; i < tokens.Count; i += 2) 
        {
            // ganjil janggal?
            if (i + 1 >= tokens.Count) break;

            string combinator = tokens[i];
            string nextSelector = tokens[i + 1];
            HashSet<DOMNode> nextNodes = new HashSet<DOMNode>();

            foreach (var node in currentNodes)
            {
                traversalNodes.Add(node);
                
                if (combinator == " ") // Descendant 
                {
                    var descendants = GetAllNodes(node, method).Skip(1);
                    foreach (var desc in descendants)
                    {
                        traversalNodes.Add(desc);
                        if (MatchesSimpleSelector(desc, nextSelector)) nextNodes.Add(desc);
                    }
                }
                else if (combinator == ">") // Child
                {
                    foreach (var child in node.Children)
                    {
                        traversalNodes.Add(child);
                        if (MatchesSimpleSelector(child, nextSelector)) nextNodes.Add(child);
                    }
                }
                else if (combinator == "+") // Adjacent Sibling
                {
                    var sibling = GetNextElementSibling(node);
                    if (sibling != null && MatchesSimpleSelector(sibling, nextSelector))
                    {
                        traversalNodes.Add(sibling);
                        nextNodes.Add(sibling);
                    }
                }
                else if (combinator == "~") // General Sibling
                {
                    var siblings = GetNextElementSiblings(node);
                    foreach (var sibling in siblings)
                    {
                        traversalNodes.Add(sibling);
                        if (MatchesSimpleSelector(sibling, nextSelector)) nextNodes.Add(sibling);
                    }
                }
            }
            
            currentNodes = nextNodes.ToList();
        }

        return (currentNodes, traversalNodes);
    }

    private static List<DOMNode> GetAllNodes(DOMNode startNode, TraversalMethod method)
    {
        return method == TraversalMethod.DFS 
            ? GetNodesDFS(startNode) 
            : GetNodesBFS(startNode);
    }

    private static List<DOMNode> GetNodesDFS(DOMNode node)
    {
        var result = new List<DOMNode>();
        TraverseDFS(node, result);
        return result;
    }

    private static void TraverseDFS(DOMNode node, List<DOMNode> result)
    {
        result.Add(node);
        foreach (var child in node.Children)
        {
            TraverseDFS(child, result);
        }
    }

    private static List<DOMNode> GetNodesBFS(DOMNode startNode)
    {
        var result = new List<DOMNode>();
        var queue = new Queue<DOMNode>();
        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);

            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }

        return result;
    }

    private static bool MatchesSimpleSelector(DOMNode node, string selector)
    {
        if (node.IsTextNode()) return false; 
        if (selector == "*") return true;    

        string? tag = null;
        string? id = null;
        List<string> classes = new List<string>();

        int i = 0;
        while (i < selector.Length)
        {
            if (selector[i] == '#') // ID 
            {
                int start = ++i;
                while (i < selector.Length && selector[i] != '#' && selector[i] != '.') i++;
                id = selector.Substring(start, i - start);
            }
            else if (selector[i] == '.') // Class
            {
                int start = ++i;
                while (i < selector.Length && selector[i] != '#' && selector[i] != '.') i++;
                classes.Add(selector.Substring(start, i - start));
            }
            else // Tag
            {
                int start = i;
                while (i < selector.Length && selector[i] != '#' && selector[i] != '.') i++;
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

        return true;
    }

    private static List<string> TokenizeSelector(string selector)
    {
        string spaced = Regex.Replace(selector, @"([>+~])", " $1 "); // change a~a to a ~ a
        string[] parts = spaced.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); // parse to a, ~, a
        
        List<string> tokens = new List<string>();
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == ">" || parts[i] == "+" || parts[i] == "~")
            {
                tokens.Add(parts[i]);
            }
            else
            {
                if (tokens.Count > 0 && tokens.Last() != ">" && tokens.Last() != "+" && tokens.Last() != "~")
                {
                    tokens.Add(" "); 
                }
                tokens.Add(parts[i]);
            }
        }
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
                if (!siblings[i].IsTextNode()) result.Add(siblings[i]);
            }
        }
        return result;
    }
}