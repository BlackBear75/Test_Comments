using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Test_Comments.Models.RecordModels;
using Test_Comments.Services;

[ApiController]
[Route("api/[controller]")]
public class RecordController : ControllerBase
{
    private readonly IRecordService _recordService;
    private readonly IUserService _userService;

    public RecordController(IRecordService recordService, IUserService userService)
    {
        _recordService = recordService;
        _userService = userService;
    }

    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddRecord([FromBody] RecordRequest request)
    {
        var storedCaptcha = HttpContext.Session.GetString("CaptchaCode");
        if (string.IsNullOrWhiteSpace(storedCaptcha) || storedCaptcha != request.Captcha)
        {
            return BadRequest(new Response { Success = false, Message = "Невірна CAPTCHA" });
        }

        var userId = User.FindFirst("userId")?.Value;
        if (userId == null)
        {
            return Unauthorized(new Response { Success = false, Message = "Невідомий користувач" });
        }

        var user = await _userService.GetUserAsync(Guid.Parse(userId));
        if (user == null)
        {
            return NotFound(new Response { Success = false, Message = "Користувача не знайдено" });
        }

        var record = new Record
        {
            UserName = user.Name,
            Email = user.Email,
            Text = request.Text,
        };

        var result = await _recordService.AddRecordAsync(record);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{recordId}/add-comment")]
    public async Task<IActionResult> AddComment(Guid recordId, [FromBody] CommentRequest request)
    {
        var userId = User.FindFirst("userId")?.Value; 
        if (userId == null)
        {
            return Unauthorized("Невідомий користувач");
        }
        var user = await _userService.GetUserAsync(Guid.Parse(userId));
        if (user == null)
        {
            return NotFound("Користувача не знайдено");
        }

        var result = await _recordService.AddCommentAsync(recordId, request.Text, user.Name, user.Email, request.ParentRecordId);
        if (result.Success)
        {
            return Ok(result); 
        }

        return BadRequest(result.Message);
    }


    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedRootRecordsWithComments(int page = 1, int pageSize = 25)
    {
        int skip = (page - 1) * pageSize;
        var records = await _recordService.GetPagedRootRecordsWithCommentsAsync(skip, pageSize);
        return Ok(records);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetTotalRootRecordsCount()
    {
        var totalCount = await _recordService.GetTotalRootRecordsCountAsync();
        return Ok(totalCount);
    }
    public class CommentRequest
{
    [Required]
    public string Text { get; set; }

    public Guid? ParentRecordId { get; set; }

}

}
