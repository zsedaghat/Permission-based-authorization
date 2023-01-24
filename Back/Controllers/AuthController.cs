using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpaceManagment.Common;
using SpaceManagment.Model;
using SpaceManagment.Service;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace SpaceManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly RoleManager<Role> _roleManager;
        public AuthController(IJwtService jwtService, IUserService userService, IAccountService accountService, RoleManager<Role> roleManager)
        {
            _userService = userService;
            _accountService = accountService;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userService.GetByName(username);
            if (user == null)
                throw new BadHttpRequestException("نام کاربری یا رمز عبور اشتباه است");
            var isPasswordValid = await _userService.CheckPasswordValid(user, password);
            if (!isPasswordValid)
                throw new BadHttpRequestException("نام کاربری یا رمز عبور اشتباه است");
            var result = await _accountService.SignIn(user, password);
            if (result.Succeeded)
            {
                var token =await _jwtService.CreateToken(user);
               // var refreshToken = _jwtService.GenerateRefreshToken();
               SetRefreshToken(token, user);
                return Ok(token);
            }
            return NotFound();
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }



        [Authorize]
        [HttpGet("AddRole")]
        public async Task<IActionResult> AddRole()
        {
            var result2 = await _roleManager.CreateAsync(new Role
            {
                Name = "Admin",
            });
            return Ok(result2);
        }
    }
}
