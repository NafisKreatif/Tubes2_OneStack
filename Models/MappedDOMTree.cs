namespace DOMTreeTraversal.Models;

public class MappedDomTree
{
    public int RootId { get; set; }
    public int MaxLevel { get; set; }
    public Dictionary<int, MappedDomNode> Nodes { get; set; }
    public MappedDomTree()
    {
        RootId = 0;
        MaxLevel = 0;
        Nodes = [];
    }
    public MappedDomTree(DOMTree tree)
    {
        RootId = tree.Root.NodeId;
        MaxLevel = 0;
        Nodes = [];
        Queue<DOMNode> bfs = new();
        Dictionary<DOMNode, int> level = [];
        bfs.Enqueue(tree.Root);
        while (bfs.Count > 0)
        {
            DOMNode node = bfs.Dequeue();
            MappedDomNode jsonable = new()
            {
                Index = node.NodeId,
                Tag = node.Tag,
                Text = node.Text,
                Id = node.Id,
                Class = [.. node.Class],
                Attribute = node.Attribute,
                Children = []
            };
            if (node.Text.Length > 0)
            {
                // Console.WriteLine(node.Text);
            }
            if (node.Parent != null)
            {
                jsonable.Parent = node.Parent.NodeId;
                Nodes[jsonable.Parent].Children.Add(node.NodeId);
                level[node] = level[node.Parent] + 1;
                MaxLevel = Math.Max(MaxLevel, level[node]);
            }
            else
            {
                jsonable.Parent = -1;
                level[node] = 0;
                MaxLevel = Math.Max(MaxLevel, level[node]);
            }

            Nodes[jsonable.Index] = jsonable;
            foreach (var child in node.Children)
            {
                bfs.Enqueue(child);
            }
        }
    }
}

public class MappedDomNode
{
    public int Index { get; set; }
    public string Tag { get; set; }
    public string Text { get; set; }
    public string Id { get; set; }
    public List<string> Class { get; set; }
    public Dictionary<string, string> Attribute { get; set; }
    public List<int> Children { get; set; }
    public int Parent { get; set; }

    public MappedDomNode()
    {
        Index = -1;
        Tag = "";
        Text = "";
        Id = "";
        Class = [];
        Attribute = [];
        Children = [];
    }
}