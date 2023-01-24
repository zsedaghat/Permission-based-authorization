using Microsoft.AspNetCore.Identity;
using SpaceManagment.Model;

namespace SpaceManagment.Service
{
    public interface IAccountService
    {
        Task<SignInResult> SignIn(User user, string password);
    }
}
