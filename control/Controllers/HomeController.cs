using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using control.Models;
using control.Services;

namespace control.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FirebaseService _firebaseService;

    public HomeController(ILogger<HomeController> logger, FirebaseService firebaseService)
    {
        _logger = logger;
        _firebaseService = firebaseService;
    }

    async public Task<IActionResult> Index()
    
    {
        List<string> tokens = await _firebaseService.GetTokensAsync();
        return View(tokens);
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
