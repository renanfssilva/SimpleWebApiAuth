using SimpleWebApiAuth.Application.Services.Auth;
using SimpleWebApiAuth.Infrastructure.Auth;

namespace SimpleWebApiAuth.API.Configuration
{
    public static class ServicesConfiguration
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
            builder.Services.AddTransient<ITokenService, TokenService>();
            builder.Services.AddTransient<IUsersService, UsersService>();
        }
    }
}
