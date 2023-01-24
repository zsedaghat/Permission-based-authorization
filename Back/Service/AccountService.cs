using Microsoft.AspNetCore.Identity;
using SpaceManagment.Model;

namespace SpaceManagment.Service
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<User> _signInManager;
        public AccountService(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<SignInResult> SignIn(User user, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(user, password, true, false);
            return result;
        }
    }
}
