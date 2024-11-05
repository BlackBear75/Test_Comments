using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Test_Comments.Entities.UserGroup;
using Test_Comments.Models.AuthModels;
using System.Threading.Tasks;
using Test_Comments.Entities.UserGroup.Repository;

namespace Test_Comments.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest request);
        Task<AuthResult> LoginAsync(LoginRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository<User> _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository<User> userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            var existingUserbyEmail = await _userRepository.FindOneAsync(u => u.Email == request.Email);
            if (existingUserbyEmail != null)
                return new AuthResult { IsSuccess = false, Message = "Користувач з такою електронною скринькою вже існує" };

            var existingUserbyName = await _userRepository.FindOneAsync(u => u.UserName == request.UserName);
            if (existingUserbyName != null)
                return new AuthResult { IsSuccess = false, Message = "Користувач з таким логіном вже існує" };

            var passwordHash = HashPassword(request.Password);

            var newUser = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = passwordHash
            };

            await _userRepository.InsertOneAsync(newUser);
            return new AuthResult { IsSuccess = true, Message = "Реєстрація успішна" };
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.FindOneAsync(u => u.Email == request.Email);
            if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash))
                return new AuthResult { IsSuccess = false, Message = "Невірний email або пароль" };

            var token = GenerateJwtToken(user);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Логін успішний",
                Token = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            string hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }
    }
}
