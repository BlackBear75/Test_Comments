using System.Threading.Tasks;
using Test_Comments.Entities.UserGroup;
using Test_Comments.Entities.UserGroup.Repository;
using Test_Comments.Models.UserModels;

namespace Test_Comments.Services;

public interface IUserService
{
    Task<UserProfileRequest> GetProfileAsync(string userId);
}

public class UserService : IUserService
{
    private readonly IUserRepository<User> _userRepository;

    public UserService(IUserRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileRequest> GetProfileAsync(string userId)
    {
        var user = await _userRepository.FindOneAsync(u => u.Id == Guid.Parse(userId));

        if (user == null)
            return null;

        return new UserProfileRequest
        {
            Name = user.UserName,
            Email = user.Email,
        };
    }
}