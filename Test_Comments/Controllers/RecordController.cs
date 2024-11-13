using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Test_Comments.Models;
using Test_Comments.Models.RecordModels;
using Test_Comments.Services;

[ApiController]
[Route("api/[controller]")]
public class RecordController : ControllerBase
{
    private readonly IRecordService _recordService;

    public RecordController(IRecordService recordService)
    {
        _recordService = recordService;
    }

    [Authorize]
    [HttpPost("add")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> AddRecord([FromForm] RecordRequest request, [FromForm] IFormFile? file, [FromForm] Guid? parentRecordId = null)
    {
        var userId = User.FindFirst("userId")?.Value;
        if (userId == null)
        {
            return Unauthorized(new Response { Success = false, Message = "Невідомий користувач" });
        }

        var result = await _recordService.AddRecordAsync(request, file, Guid.Parse(userId), parentRecordId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedRootRecordsWithComments(int page = 1, int pageSize = 25, string sortField = "creationDate", string sortDirection = "asc")
    {
        var records = await _recordService.GetPagedRootRecordsWithCommentsAsync(page, pageSize, sortField, sortDirection);
        return Ok(records);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetTotalRootRecordsCount()
    {
        var totalCount = await _recordService.GetTotalRootRecordsCountAsync();
        return Ok(totalCount);
    }
    
}
