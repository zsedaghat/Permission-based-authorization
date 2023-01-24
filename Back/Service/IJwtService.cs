using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SpaceManagment.Common;
using SpaceManagment.Model;
using System.Security.Claims;

namespace SpaceManagment.Service
{
    public interface IJwtService
    {
        Task<RefreshToken> CreateToken(User user);
        Task<IEnumerable<Claim>> GetClaimsAsync(User user);
        RefreshToken GenerateRefreshToken();

    }
}
