using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Jogging.Domain.Configuration;
using Jogging.Domain.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Jogging.Rest.Utils;

public static class JwtTokenUtil
{
    public static string Generate(JwtConfiguration configuration, int userId, string role) {
        var guid = Guid.NewGuid().ToString();

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Key));
        var signInCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var claims = new[]
        {
        new Claim("UserId", userId.ToString()),
        new Claim("Guid", guid),
        new Claim(ClaimTypes.Role, role)
    };

        var securityToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddDays(30),
            issuer: configuration.Issuer,
            audience: configuration.Audience,
            signingCredentials: signInCredentials,
            claims: claims
        );

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(securityToken);

        return tokenString;
    }

    public static int GetUserIdFromToken(HttpContext httpContext, bool throwError = true)
    {
        try
        {
            var token = GetTokenFromCookie(httpContext);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            AuthenticationValidator.ValidateJwtToken(jwtToken.ToString());

            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "UserId");

            return AuthenticationValidator.ValidateUserId(userIdClaim?.Value);
        }
        catch (Exception)
        {
            if (throwError)
            {
                throw;
            }
            else
            {
                return -1;
            }
        }
    }


    public static Guid GetGuIdFromToken(HttpContext httpContext)
    {
        var token = GetTokenFromCookie(httpContext);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

        AuthenticationValidator.ValidateJwtToken(jwtToken.ToString());

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "Guid");

        return AuthenticationValidator.ValidateGuId(userIdClaim?.Value);
    }
    public static string GetRoleFromToken(HttpContext httpContext)
    {
        var token = httpContext.Request.Cookies["X-Access-Token"];

        if (token != null)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            var userRoleClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

            if (userRoleClaim != null)
            {
                return userRoleClaim.Value;
            }
        }

        return "User";
    }


    public static string GetTokenFromCookie(HttpContext httpContext)
    {
        var token = httpContext.Request.Cookies["X-Access-Token"];
        AuthenticationValidator.ValidateJwtToken(token);
        return token;
    }

    public static string GetTokenFromHeader(HttpContext httpContext)
    {
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        AuthenticationValidator.ValidateJwtToken(token);
        return token;
    }
}