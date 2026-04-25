namespace DOMTreeTraversal.Models;

using System;
using System.Collections.Generic;

//2 Jenis DOMNode: Tag atau Text
public class DOMNode(string tag = "", string text = "", int id = -1)
{
    private static int IdCounter = 1;
    public int NodeId { get; set; } = id == -1 ? IdCounter++ : id;

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

    public DOMTree(MappedDomTree mappedDomTree)
    {
        MappedDomNode root = new();
        foreach (var item in mappedDomTree.Nodes)
        {
            if (item.Value.Parent == -1)
            {
                root = item.Value;
            }
        }

        Dictionary<int, DOMNode> nodes = [];
        Queue<int> bfs = new();
        bfs.Enqueue(root.Index);
        Root = new DOMNode()
        {
            NodeId = root.Index,
            Tag = root.Tag,
            Id = root.Id,
            Class = root.Class,
            Attribute = root.Attribute,
            Parent = null,
            Text = root.Text,
            Children = []
        };
        nodes[root.Index] = Root;
        while (bfs.Count > 0)
        {
            int currentIndex = bfs.Dequeue();
            MappedDomNode currentMappedNode = mappedDomTree.Nodes[currentIndex];
            foreach (var childIndex in currentMappedNode.Children)
            {
                MappedDomNode childMappedNode = mappedDomTree.Nodes[childIndex];
                DOMNode childNode = new()
                {
                    NodeId = childIndex,
                    Tag = childMappedNode.Tag,
                    Id = childMappedNode.Id,
                    Class = childMappedNode.Class,
                    Attribute = childMappedNode.Attribute,
                    Parent = nodes[currentIndex],
                    Text = childMappedNode.Text,
                    Children = []
                };
                nodes[childIndex] = childNode;
                nodes[currentIndex].Children.Add(childNode);
                bfs.Enqueue(childIndex);
            }
        }
    }
}
