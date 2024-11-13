using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Test_Comments.Entities.UserGroup;
using Test_Comments.Entities.UserGroup.Repository;

namespace Test_Comments.Services
{
    public interface ICaptchaService
    {
        Task<string> GenerateCaptchaCodeAsync(Guid userId);
        byte[] GenerateCaptchaImage(string captchaCode);
        bool ValidateCaptcha(string inputCaptcha, string storedCaptcha);
    }

    public class CaptchaService : ICaptchaService
    {
        private const int Width = 200;
        private const int Height = 50;
        private const int Length = 6;

        private readonly IUserRepository<User> _userRepository;

        public CaptchaService(IUserRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> GenerateCaptchaCodeAsync(Guid userId)
        {
            string captchaCode = GenerateCaptchaCode();
            var user = await _userRepository.FindOneAsync(x => x.Id == userId);
            user.Captcha = captchaCode;
            await _userRepository.UpdateOneAsync(user);

            return captchaCode;
        }

        public byte[] GenerateCaptchaImage(string captchaCode)
        {
            using (var image = new Image<Rgba32>(Width, Height))
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
                    return ms.ToArray();
                }
            }
        }

        public bool ValidateCaptcha(string inputCaptcha, string storedCaptcha)
        {
            return !string.IsNullOrWhiteSpace(inputCaptcha) && inputCaptcha == storedCaptcha;
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
    }
}
