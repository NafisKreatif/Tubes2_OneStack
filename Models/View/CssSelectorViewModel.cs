namespace DOMTreeTraversal.Models.View;

public class CssSelectorViewModel
{
    public string? DomTreeJson;   // Input for visualized DomTree
    public string? CssSelector;   // Input for CSS selector
    public string? TraversalType; // Input for traversal type
    public int? ResultCount;      // Input for how many result should be searched
    public double? TimeSpanSecond;   // Execution time
    public string? SelectedJson;  // Selected DOM Node ID
    public string? TraversalJson; // Order of traversed DOM Node ID sequence by BFS/DFS
}
