using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Test_Comments.Base.Repository;
using Test_Comments.Entities;
using Test_Comments.Entities.CommentGroup;
using Test_Comments.Entities.CommentGroup.Repository;
using Test_Comments.Models;

namespace Test_Comments.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICommentRepository<Comment> _comment;

    public HomeController(ILogger<HomeController> logger,ICommentRepository<Comment> comment)
    {
        _logger = logger;
        _comment = comment;
    }

    
}