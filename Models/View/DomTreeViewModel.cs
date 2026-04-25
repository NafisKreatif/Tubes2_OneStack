using System.Text.Json;

namespace DOMTreeTraversal.Models.View;

public class DomTreeViewModel(DOMTree domTree)
{
    public string? DomTreeJson = JsonSerializer.Serialize(new MappedDomTree(domTree));
}