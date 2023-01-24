using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpaceManagment.Common;
using SpaceManagment.DTO;
using SpaceManagment.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SpaceManagment.Service
{
    public class JwtService: IJwtService
    {
        private readonly Settings _Setting;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
  
        public JwtService(IOptionsSnapshot<Settings> settings, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _Setting = settings.Value;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<RefreshToken> CreateToken(User user)
        {
            var claims = await GetClaimsAsync(user);
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_Setting.JwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            var refToken = GenerateRefreshToken();
            refToken.Jwt=jwt;
           await AddToken(refToken,user);
            return refToken;

            //var tokenHandler = new JwtSecurityTokenHandler();
            //var tokenKey = Encoding.UTF8.GetBytes(_Setting.JwtSettings.Key);
            //var claims = await GetClaimsAsync(user);
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(claims),
            //    Expires = DateTime.UtcNow.AddMinutes(10),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            //};

            //var token = tokenHandler.CreateToken(tokenDescriptor);



   



        }
        
        public async Task<IEnumerable<Claim>> GetClaimsAsync(User user)
        {
            var result = await _signInManager.ClaimsFactory.CreateAsync(user);
            var result2 = await _userManager.GetClaimsAsync(user);
            //add custom claims
            var list = new List<Claim>(result2);
            //list.Add(new Claim(ClaimTypes.MobilePhone, "09123456987"));
            return list;

        }

        public RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };
            return refreshToken;
        }

        public async Task AddToken(RefreshToken refreshToken, User user)
        {
            user.RefreshToken = refreshToken.Token;
           // user.token = refreshToken.Jwt;
            
            var result = await _userManager.UpdateAsync(user);
        }

       

    }
}
