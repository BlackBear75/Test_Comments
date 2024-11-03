using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Test_Comments.Controllers;

public class UserController  : Controller
{
    public IActionResult Privacy()
    {
        return View();
    }
}