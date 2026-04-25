using Newtonsoft.Json;

namespace DOMTreeTraversal.Models.View;

public class DomTreeViewModel(DOMTree domTree)
{
    public string? DomTreeJson = JsonConvert.SerializeObject(
        new MappedDomTree(domTree),
        Formatting.None,
        new JsonSerializerSettings()
        {
            StringEscapeHandling = StringEscapeHandling.EscapeHtml
        }).Replace("\\", "\\\\");
}