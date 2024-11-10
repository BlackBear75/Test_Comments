using System.Threading.Tasks;
using Test_Comments.Entities.RecordGroup.Repository;
using Test_Comments.Entities.UserGroup;
using Test_Comments.Entities.UserGroup.Repository;
using Test_Comments.Helper;
using Test_Comments.Models.UserModels;

namespace Test_Comments.Services;

public interface IUserService
{
    Task<UserProfileRequest> GetUserAsync(Guid userId);
    Task<Response> UpdateProfileAsync(Guid userId, UserProfileRequest request);
}

public class UserService : IUserService
{
    private readonly IUserRepository<User> _userRepository;
    private readonly IRecordRepository<Record> _recordRepository;

    public UserService(IUserRepository<User> userRepository,IRecordRepository<Record> recordRepository)
    {
        _userRepository = userRepository;
        _recordRepository = recordRepository;
    }

    public async Task<UserProfileRequest> GetUserAsync(Guid userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        return new UserProfileRequest
        {
            Name = user.UserName,
            Email = user.Email,
        };
    }
    public async Task<Response> UpdateProfileAsync(Guid userId, UserProfileRequest request)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            return new Response { Success = false, Message = "Користувача не знайдено" };
        }

        if (user.UserName != request.Name)
        {
            var userByName = await _userRepository.FindOneAsync(u => u.UserName == request.Name && u.Id != userId);
            if (userByName != null)
            {
                return new Response { Success = false, Message = "Цей логін вже зайнятий іншим користувачем" };
            }
        }
        if (user.Email != request.Email)
        {
            var userByEmail = await _userRepository.FindOneAsync(u => u.Email == request.Email && u.Id != userId);
            if (userByEmail != null)
            {
                return new Response { Success = false, Message = "Ця електронна адреса вже використовується іншим користувачем" };
            }
        }

        string usernameforfinding = user.UserName;
        user.UserName = HtmlHelper.SanitizeHTML(request.Name);
        user.Email = HtmlHelper.SanitizeHTML(request.Email);
        await _userRepository.UpdateOneAsync(user);

        await _recordRepository.UpdateManyAsync(
            r => r.UserName == usernameforfinding,
            record =>
            {
                record.UserName = user.UserName;
                record.Email = user.Email;
            }
        );

        return new Response { Success = true, Message = "Профіль успішно оновлено" };
    }





}