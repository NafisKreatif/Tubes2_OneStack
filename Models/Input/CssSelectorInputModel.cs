namespace DOMTreeTraversal.Models.Input;

public class CssSelectorInputModel
{
    public string? DomTreeJson { get; set; }   // Input for visualized DomTree
    public string? CssSelector { get; set; }   // Input for CSS selector
    public string? TraversalType { get; set; } // Input for traversal type
    public string? ResultCount { get; set; } // Input for how many result should be searched
}