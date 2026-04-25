using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DOMTreeTraversal.Models.View;
using DOMTreeTraversal.Models.Input;
using DOMTreeTraversal.Test;
using System.Text.Json;
using DOMTreeTraversal.Models;

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
    public IActionResult DomTreeResult([Bind("HtmlLink", "HtmlText", "InputType")] HtmlInputModel model, IFormFile htmlFile)
    {
        string html = "";
        switch (model.InputType)
        {
            case "link":
                html = model.HtmlLink ?? "";
                break;
            case "file":
                if (htmlFile != null && htmlFile.Length > 0) html = htmlFile.FileName ?? "";
                break;
            case "text":
                html = model.HtmlText ?? "";
                break;
            default:
                break;
        }

        return View(new DomTreeViewModel(TestCase1.GetDomTree()));
    }

    [HttpPost]
    public IActionResult CssSelectorResult([Bind("DomTreeJson", "CssSelector", "TraversalType")] CssSelectorInputModel model)
    {
        return View(new CssSelectorViewModel
        {
            DomTreeJson = model.DomTreeJson,
            CssSelector = model.CssSelector,
            TraversalType = model.TraversalType,
            SelectedJson = JsonSerializer.Serialize(TestCase1.GetSelectedNode()),
            TraversalJson = JsonSerializer.Serialize(TestCase1.GetTraversalOrder())
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
