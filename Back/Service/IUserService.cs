using Microsoft.AspNetCore.Identity;
using SpaceManagment.DTO;
using SpaceManagment.Model;

namespace SpaceManagment.Service
{
    public interface IUserService
    {
        Task<IdentityResult> AddUser(UserDto userDto);
        Task<IdentityResult> UpdateUser(User user, UserDto userDto);
        Task<IdentityResult> DeleteUser(string id);
        Task<List<User>> Users();
        Task<User> GetById(string id);
        Task<User> Get(UserDto userDto);
        Task<User> GetByName(string Name);
        Task<bool> CheckPasswordValid(User user, string password);
    }
}
