using Microsoft.IdentityModel.Tokens;
using SimpleWebApiAuth.Application;
using SimpleWebApiAuth.Domain.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleWebApiAuth.Tests.Helpers
{
    public static class TokenHelper
    {
        public static string GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("156dae95c5764467863a54b3248fd15e");

            var claims = user.Claims.Select(claim => new Claim(claim.Type, claim.Value)).ToList();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName!));
            claims.Add(new Claim(ClaimTypes.Role, Settings.AdminRole));
            claims.Add(new Claim(ClaimTypes.Role, Settings.CommonRole));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
