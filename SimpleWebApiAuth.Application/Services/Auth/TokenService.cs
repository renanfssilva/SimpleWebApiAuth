using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimpleWebApiAuth.Domain.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleWebApiAuth.Application.Services.Auth
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
    }

    public class TokenService(RoleManager<ApplicationRole> roleManager, IConfiguration configuration) : ITokenService
    {
        public string GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Authentication__SecretKey"))
                ? configuration.GetSection("Authentication:SecretKey").Value
                : Environment.GetEnvironmentVariable("Authentication__SecretKey");
            var key = Encoding.ASCII.GetBytes(secretKey!);

            var claims = user.Claims.Select(claim => new Claim(claim.Type, claim.Value)).ToList();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName!));
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, roleManager.FindByIdAsync(role.ToString()).Result!.ToString())));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
