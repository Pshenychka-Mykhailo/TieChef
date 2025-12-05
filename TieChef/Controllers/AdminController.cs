using Microsoft.AspNetCore.Mvc;

namespace TieChef.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
