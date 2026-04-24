using System.Text.Json;

namespace DOMTreeTraversal.Models;

public class ResultViewModel(DOMTree domTree)
{
    public string? DOMTreeJson = JsonSerializer.Serialize(domTree);
    public string? TraversalOrder = JsonSerializer.Serialize(domTree);
}

public class JsonModel(string json)
{
    public string json = json;
}
