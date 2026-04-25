namespace DOMTreeTraversal.Models;

using System;
using System.Collections.Generic;

//2 Jenis DOMNode: Tag atau Text
public class DOMNode(string tag = "", string text = "")
{
    private static int IdCounter = 1;
    public int NodeId = IdCounter++;

    // tag (<body>, </div>, </span>, dll) 
    public string Tag { get; set; } = tag;
    // teks
    // contoh : "<body>Hello</body>, Hello berarti Text"
    public string Text { get; set; } = text;
    //untuk Id di css selector
    public string Id { get; set; } = "";
    //untuk class di css selector
    public List<string> Class { get; set; } = new List<string>();
    public Dictionary<string, string> Attribute { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public List<DOMNode> Children { get; set; } = new List<DOMNode>();
    public DOMNode? Parent { get; set; }

    public bool IsTextNode()
    {
        return string.IsNullOrEmpty(Tag) && !string.IsNullOrEmpty(Text);
    }
}
public class DOMTree
{
    public DOMNode Root { get; set; }

    //constructor
    public DOMTree()
    {
        Root = new DOMNode("document");
    }


    public DOMNode AddChild(DOMNode parent, string tag, string text = "")
    {
        var node = new DOMNode(tag, text);
        node.Parent = parent;
        parent.Children.Add(node);
        return node;
    }
}
