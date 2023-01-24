using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpaceManagment.DTO;
using SpaceManagment.Model;
using System.Security.Claims;

namespace SpaceManagment.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> AddUser(UserDto userDto)
        {
            var user = new User
            {
                Age = userDto.Age,
                Gender = userDto.Gender,
                UserName = userDto.UserName,
                Email = userDto.Email
            };
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"{result.Errors}");
            }
            await AddClaim(user);
            return result;
        }

        public async Task<IdentityResult> UpdateUser(User user, UserDto userDto)
        {
            user.UserName = userDto.UserName;
            user.Gender = userDto.Gender;
            user.Email = userDto.Email;
            user.Age = userDto.Age;
            user.PasswordHash = userDto.Password;
            var result = await _userManager.UpdateAsync(user);

            var oldClaims = await _userManager.GetClaimsAsync(user);
            await _userManager.RemoveClaimsAsync(user, oldClaims);
            var newClaims = await AddClaim(user);

            return result;
        }

        public async Task<IdentityResult> DeleteUser(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null)
                new Exception("کاربری با این مشخصات موجود نمیباشد");
            var result = await _userManager.DeleteAsync(user);
            return result;
        }

        public async Task<List<User>> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user;
        }

        public async Task<User> Get(UserDto userDto)
        {
            var user = await _userManager.FindByNameAsync(userDto.UserName);
            return user;
        }

        public async Task<User> GetByName(string Name)
        {
            var user = await _userManager.FindByNameAsync(Name);
            return user;
        }

        public async Task<bool> CheckPasswordValid(User user, string password)
        {
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            return isPasswordValid;
        }


        private async Task<List<Claim>> AddClaim(User user)
        {
            var claims = await _signInManager.ClaimsFactory.CreateAsync(user);
            var claimslist = new List<Claim>(claims.Claims);
            await _userManager.AddClaimsAsync(user, claimslist);
            return claimslist;
        }
    }
}
