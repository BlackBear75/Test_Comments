using System.Threading.Tasks;
using Test_Comments.Entities.UserGroup;
using Test_Comments.Entities.UserGroup.Repository;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using Test_Comments.Models.AuthModels;

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

        public AuthService(IUserRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            
            var existingUserbyEmail = await _userRepository.FindOneAsync(u => u.Email == request.Email);
            if (existingUserbyEmail != null)
                return new AuthResult { IsSuccess = false, Message = "Користувач з такой електронной скринькой вже існує" };
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

            return new AuthResult 
            { 
                IsSuccess = true, 
                Message = "Логін успішний", 
                UserId = user.Id 
            };
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
