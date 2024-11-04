using Microsoft.AspNetCore.Mvc;
using Test_Comments.Models.AuthModels;
using Test_Comments.Services;

namespace Test_Comments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(request);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result.IsSuccess)
                return Ok(result);
            return Unauthorized(result);
        }
    }
}