using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DOMTreeTraversal.Models;
using DOMTreeTraversal.Test;
using System.Text.Json;

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
    public IActionResult Result([Bind("link", "htmlText", "inputType", "traversalType")] InputModel model, IFormFile htmlFile)
    {
        Console.WriteLine(model.inputType);
        string html = "";
        switch (model.inputType)
        {
            case "link":
                html = model.link ?? "";
                break;
            case "file":
                if (htmlFile != null && htmlFile.Length > 0) html = htmlFile.FileName ?? "";
                break;
            case "text":
                html = model.htmlText ?? "";
                break;
            default:
                break;
        }

        JsonableDomTree test = new JsonableDomTree(DOMTreeStub.GetTestCase1());
        for (int i = 0; i < test.Nodes.Count; i++)
        {
            Console.WriteLine(i + " : " + test.Nodes[i]);
        Console.WriteLine(JsonSerializer.Serialize(test.Nodes));
        }
        return View(new JsonModel(JsonSerializer.Serialize(test.Nodes)));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
