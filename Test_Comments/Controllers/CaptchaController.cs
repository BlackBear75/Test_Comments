using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
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
            
            
            using (Bitmap bitmap = new Bitmap(Width, Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    Font font = new Font("Arial", 24);
                    g.DrawString(captchaCode, font, Brushes.Black, 10, 10);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }

        private string GenerateCaptchaCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder captchaCode = new StringBuilder();
            Random random = new Random();

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
