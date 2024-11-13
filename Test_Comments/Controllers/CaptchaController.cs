using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Test_Comments.Entities.UserGroup;
using Test_Comments.Entities.UserGroup.Repository;
using Test_Comments.Services;

namespace Test_Comments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaptchaController : ControllerBase
    {
        private readonly ICaptchaService _captchaService;
        private readonly IUserRepository<User> _userRepository;

        public CaptchaController(ICaptchaService captchaService, IUserRepository<User> userRepository)
        {
            _captchaService = captchaService;
            _userRepository = userRepository;
        }

        [HttpGet("generate")]
        public async Task<IActionResult> Generate()
        {
            var userId = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Невідомий користувач" });

            var captchaCode = await _captchaService.GenerateCaptchaCodeAsync(Guid.Parse(userId));
            var captchaImage = _captchaService.GenerateCaptchaImage(captchaCode);

            return File(captchaImage, "image/png");
        }
        
    }
}