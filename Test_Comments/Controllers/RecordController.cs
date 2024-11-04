using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Test_Comments.Models.RecordModels;
using Test_Comments.Services;

namespace Test_Comments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordController : ControllerBase
    {
        private readonly IRecordService _recordService;

        public RecordController(IRecordService recordService)
        {
            _recordService = recordService;
        }

     
        [HttpPost("add")]
        public async Task<IActionResult> AddRecord([FromBody] RecordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Captcha))
            {
                return BadRequest(new Response { Success = false, Message = "CAPTCHA обов'язкова" });
            }

            var result = await _recordService.AddRecordAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}