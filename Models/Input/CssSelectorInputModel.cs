namespace DOMTreeTraversal.Models.Input;

public class CssSelectorInputModel
{
    public string? DomTreeJson { get; set; }   // Input for visualized DomTree
    public string? CssSelector { get; set; }   // Input for CSS selector
    public string? TraversalType { get; set; } // Input for traversal type
}