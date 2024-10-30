using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Test_Comments.Base.Repository;
using Test_Comments.Entities;
using Test_Comments.Entities.CommentsGroup;
using Test_Comments.Entities.CommentsGroup.Repository;
using Test_Comments.Models;

namespace Test_Comments.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICommentsRepository<Comment> _comment;

    public HomeController(ILogger<HomeController> logger,ICommentsRepository<Comment> comment)
    {
        _logger = logger;
        _comment = comment;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            Comment comment = new Comment()
            {
                UserName = "Stepan",
                Email = "dasf@fd.co",
                HomePage = "http://example.com",
                Captcha = "ABCD1234",
                Text = "Це текст коментаря."
            };

            await _comment.InsertOneAsync(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while inserting comment.");
            return StatusCode(500, "Internal server error.");
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