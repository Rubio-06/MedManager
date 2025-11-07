using System.Diagnostics;

using MedManager.Web.Helpers;
using MedManager.Web.Models;

using Microsoft.AspNetCore.Mvc;

namespace MedManager.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Si l'utilisateur est connecté, rediriger vers son dashboard
        if (User.Identity?.IsAuthenticated == true)
        {
            var dashboardRoute = UserHelper.GetDashboardRoute(User);
            return Redirect(dashboardRoute);
        }

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
}
