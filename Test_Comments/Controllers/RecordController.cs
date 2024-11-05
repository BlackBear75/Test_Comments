using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Test_Comments.Models.RecordModels;
using Test_Comments.Services;

namespace Test_Comments.Controllers
{
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
            if (string.IsNullOrWhiteSpace(request.Captcha))
            {
                return BadRequest(new Response { Success = false, Message = "CAPTCHA обов'язкова" });
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

            var recordModel = new RecordModel()
            {
                UserName = user.Name,
                Email = user.Email,
                Text = request.Text,
                Captcha = request.Captcha
            };
            
            var result = await _recordService.AddRecordAsync(recordModel);

            if (result.Success)return Ok(result);
            
            return BadRequest(result);
            
        }
    }
}