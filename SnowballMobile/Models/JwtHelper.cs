using System.IdentityModel.Tokens.Jwt;

namespace SnowballMobile.Models;

public static class JwtHelper
{
    public static string? GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return jwt.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
    }

    public static bool HasRole(string token, string role)
    {
        if (string.IsNullOrEmpty(token)) return false;
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        return jwt.Claims
            .Where(c => c.Type == "role")
            .Any(c => string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase));
    }
}