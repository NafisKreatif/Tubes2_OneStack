namespace DOMTreeTraversal.Test;

using DOMTreeTraversal.Models;

class DOMTreeStub
{
    public static DOMTree GetTestCase1()
    {
        DOMTree tree = new();
        DOMNode node = tree.Root;
        tree.AddChild(node, "head");
        DOMNode body = tree.AddChild(node, "body");
        DOMNode div1 = tree.AddChild(body, "div");
        DOMNode div2 = tree.AddChild(body, "div");
        tree.AddChild(div1, "h1", "Nafis Suka Matematika");
        tree.AddChild(div1, "p", "Lorem lorem ipsum apa yang kalo dikali dua hasilnya enam?");
        tree.AddChild(div2, "h2", "Ini tes doang");
        node = tree.AddChild(div2, "div");
        tree.AddChild(node, "button", "Click here!");

        return tree;
    }
}