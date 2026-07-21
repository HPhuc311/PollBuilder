using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PollBuilder.Web.Models;

namespace PollBuilder.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult LoginRequired(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
}
