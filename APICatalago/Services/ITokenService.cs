using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalago.Services
{
    public interface ITokenService
    {
        JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config);
        string GenerateRefreshToken();
        ClaimsPrincipal GetMainFromExpiredToken(string token, IConfiguration _config);
    }
}
