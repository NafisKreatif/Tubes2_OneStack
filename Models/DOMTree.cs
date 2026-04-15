namespace DOMTreeTraversal.Models;
using System;
using System.Collections.Generic;

//2 Jenis DOMNode: Tag atau Text
public class DOMNode 
{
    
    // tag (<body>, </div>, </span>, dll) 
    public string Tag { get; set; }
    // teks
    // contoh : "<body>Hello</body>, Hello berarti Text"
    public string Text { get; set; }
    //untuk Id di css selector
    public string Id{get; set;}
    //untuk class di css selector
    public List<string> Class {get; set; }
    public List<DOMNode> Children { get; set; }
    public DOMNode Parent { get; set; }
    //constructor
    public DOMNode(string tag = "", string text = "")
    {
        Tag = tag;
        Text = text;
        Children = new List<DOMNode>();
    }
    
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