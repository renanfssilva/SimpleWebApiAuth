using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SimpleWebApiAuth.API.Configuration
{
    public static class AuthenticationConfiguration
    {
        public static void ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            var secret = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Authentication__SecretKey"))
                ? builder.Configuration.GetSection("Authentication:SecretKey").Value
                : Environment.GetEnvironmentVariable("Authentication__SecretKey");

            var key = Encoding.ASCII.GetBytes(secret!);
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        }
    }
}
