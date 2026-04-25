using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DOMTreeTraversal.Models.View;
using DOMTreeTraversal.Models.Input;
using DOMTreeTraversal.Test;
using DOMTreeTraversal.Models;
using Newtonsoft.Json;

namespace DOMTreeTraversal.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DomTreeResult([Bind("HtmlLink", "HtmlText", "InputType")] HtmlInputModel model, IFormFile htmlFile)
    {
        DOMTree domTree = new();
        try
        {
            switch (model.InputType)
            {
                case "link":
                    domTree = await HTMLParser.ParseFromUrl(model.HtmlLink ?? "");
                    break;
                case "file":
                    if (htmlFile != null && htmlFile.Length > 0)
                    {
                        using var reader = new StreamReader(htmlFile.OpenReadStream());
                        string htmlContent = await reader.ReadToEndAsync();
                        domTree = HTMLParser.ParseFromString(htmlContent);
                    }
                    break;
                case "text":
                    domTree = HTMLParser.ParseFromString(model.HtmlText ?? "");
                    break;
            }
        }
        catch (Exception e)
        {
            domTree = new();
            Console.WriteLine(e.ToString());
        }

        return View(new DomTreeViewModel(domTree));
    }

    [HttpPost]
    public IActionResult CssSelectorResult([Bind("DomTreeJson", "CssSelector", "TraversalType")] CssSelectorInputModel model)
    {
        MappedDomTree domTree;

        if (model.DomTreeJson != null)
        {
            domTree = JsonConvert.DeserializeObject<MappedDomTree>(model.DomTreeJson) ?? new MappedDomTree();
        }
        else
        {
            domTree = new MappedDomTree();
        }
        List<int> selectedNode = new();
        List<int> traversedNode = new();
        foreach (var item in domTree.Nodes)
        {
            if (item.Value.Parent == -1) continue;
            if (Random.Shared.NextInt64() % 4 == 0)
            {
                selectedNode.Add(item.Key);
            }
            if (Random.Shared.NextInt64() % 2 == 0)
            {
                traversedNode.Add(item.Key);
            }
        }
        return View(new CssSelectorViewModel
        {
            DomTreeJson = model.DomTreeJson,
            CssSelector = model.CssSelector,
            TraversalType = model.TraversalType,
            ResultCount = model.ResultCount,
            SelectedJson = JsonConvert.SerializeObject(selectedNode),
            TraversalJson = JsonConvert.SerializeObject(traversedNode)
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
