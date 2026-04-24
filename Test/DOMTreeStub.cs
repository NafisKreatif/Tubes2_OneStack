namespace DOMTreeTraversal.Test;

using DOMTreeTraversal.Models;

class DOMTreeStub
{
    public static DOMTree GetTestCase1()
    {
        DOMTree tree = new();
        DOMNode node = tree.Root;
        node = tree.AddChild(node, "head");
        node = tree.AddChild(node, "body");
        tree.AddChild(node, "h1", "Judul");
        tree.AddChild(node, "p", "Lorem lorem ipsum");

        return tree;
    }
}