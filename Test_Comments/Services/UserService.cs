using System.Threading.Tasks;
using Test_Comments.Entities.UserGroup;
using Test_Comments.Entities.UserGroup.Repository;
using Test_Comments.Models.UserModels;

namespace Test_Comments.Services;

public interface IUserService
{
    Task<UserProfileRequest> GetUserAsync(Guid userId);
    Task<bool> UpdateProfileAsync(Guid userId, UserProfileRequest request);
}

public class UserService : IUserService
{
    private readonly IUserRepository<User> _userRepository;

    public UserService(IUserRepository<User> userRepository)
    {
        _userRepository = userRepository;
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
    public async Task<bool> UpdateProfileAsync(Guid userId, UserProfileRequest request)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        user.UserName = request.Name;
        user.Email = request.Email;
        await _userRepository.UpdateOneAsync(user);

        return true;
    }

}