namespace DOMTreeTraversal.Test;

using DOMTreeTraversal.Models;

class TestCase1
{
    public static DOMTree GetDomTree()
    {
        DOMTree tree = new();
        DOMNode node = tree.Root;
        node = tree.AddChild(node, "html");
        DOMNode head = tree.AddChild(node, "head");
        tree.AddChild(head, "script");
        DOMNode body = tree.AddChild(node, "body");
        body.Class.AddRange("root", "container", "mx-auto");
        DOMNode div1 = tree.AddChild(body, "div");
        DOMNode div2 = tree.AddChild(body, "div");
        tree.AddChild(div1, "h1", "Nafis Suka Matematika");
        tree.AddChild(div1, "p", "Lorem lorem ipsum apa yang kalo dikali dua hasilnya enam?");
        tree.AddChild(div2, "h2", "Ini tes doang");
        node = tree.AddChild(div2, "div");
        node.Id = "playButtons";
        node.Class.Add("button-container");

        tree.AddChild(node, "button", "Click here!");
        tree.AddChild(node, "button", "Click here!");
        tree.AddChild(node, "button", "Click here!");

        return tree;
    }

    public static List<int> GetTraversalOrder()
    {
        return [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    }

    public static List<int> GetSelectedNode()
    {
        return [3, 5, 8, 9];
    }
}