using Microsoft.AspNetCore.Mvc;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Text;

namespace Test_Comments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaptchaController : ControllerBase
    {
        private const int Width = 200;
        private const int Height = 50;
        private const int Length = 6; 

        [HttpGet("generate")]
        public IActionResult Generate()
        {
            string captchaCode = GenerateCaptchaCode();
            HttpContext.Session.SetString("CaptchaCode", captchaCode);
            
            using (Image<Rgba32> image = new Image<Rgba32>(Width, Height))
            {
                image.Mutate(ctx =>
                {
                    ctx.Fill(Color.White); 
                    var font = SystemFonts.CreateFont("DejaVu Sans", 24);

                    ctx.DrawText(captchaCode, font, Color.Black, new PointF(10, 10)); 
                });

                using (var ms = new MemoryStream())
                {
                    image.SaveAsPng(ms); 
                    return File(ms.ToArray(), "image/png"); 
                }
            }
        }

        private string GenerateCaptchaCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var captchaCode = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < Length; i++)
            {
                captchaCode.Append(chars[random.Next(chars.Length)]);
            }

            return captchaCode.ToString();
        }

        [HttpPost("validate")]
        public IActionResult Validate([FromBody] string captchaInput)
        {
            var storedCaptcha = HttpContext.Session.GetString("CaptchaCode");

            if (string.IsNullOrWhiteSpace(captchaInput) || captchaInput != storedCaptcha)
            {
                return BadRequest(new { Success = false, Message = "Невірна CAPTCHA" });
            }

            return Ok(new { Success = true, Message = "CAPTCHA вірна" });
        }
    }
}
