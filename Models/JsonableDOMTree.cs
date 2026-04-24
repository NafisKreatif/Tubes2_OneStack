using DOMTreeTraversal.Models;

class JsonableDomTree
{
    public List<JsonableDomNode> Nodes;
    public JsonableDomTree(DOMTree tree)
    {
        Nodes = [];
        Queue<DOMNode> bfs = new();
        bfs.Enqueue(tree.Root);
        while (bfs.Count > 0)
        {
            DOMNode node = bfs.Dequeue();
            JsonableDomNode jsonable = new()
            {
                Index = node.NodeID,
                Tag = node.Tag,
                Text = node.Text,
                Id = node.Id,
                Class = [.. node.Class],
                Attribute = node.Attribute,
                Children = []
            };
            if (node.Parent != null)
            {
                jsonable.Parent = node.Parent.NodeID;
                Nodes[jsonable.Parent].Children.Add(node.NodeID);
            }
            else
            {
                jsonable.Parent = -1;
            }

            Nodes.Add(jsonable);
            foreach (var child in node.Children)
            {
                bfs.Enqueue(child);
            }
        }
    }
}

class JsonableDomNode
{
    public int Index { get; set; }
    public string Tag { get; set; }
    public string Text { get; set; }
    public string Id { get; set; }
    public List<string> Class { get; set; }
    public Dictionary<string, string> Attribute { get; set; }
    public List<int> Children { get; set; }
    public int Parent { get; set; }

    public JsonableDomNode()
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