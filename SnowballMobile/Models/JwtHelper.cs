using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace SnowballMobile.Models;

public static class JwtHelper
{
    public static string? GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return jwt.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
    }
}