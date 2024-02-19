using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using SimpleWebApiAuth.Application;
using SimpleWebApiAuth.Domain.Identity;

namespace SimpleWebApiAuth.API.Configuration
{
    public static class AuthorizationConfiguration
    {
        public static void ConfigureAuthorization(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("Administrator", policy => policy.RequireRole(Settings.AdminRole))
                .AddPolicy("User", policy => policy.RequireRole(Settings.CommonRole));

            var connectionString = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("UserDatabase__ConnectionString"))
                ? builder.Configuration.GetConnectionString("DefaultConnection")
                : Environment.GetEnvironmentVariable("UserDatabase__ConnectionString");

            var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
            {
                MongoDbSettings = new MongoDbSettings
                {
                    ConnectionString = connectionString,
                    DatabaseName = builder.Configuration.GetSection("UserDatabase:DatabaseName").Value
                },
                IdentityOptionsAction = identity =>
                {
                    identity.Password.RequireDigit = true;
                    identity.Password.RequiredLength = 6;
                    identity.Password.RequireLowercase = false;
                    identity.Password.RequireUppercase = false;
                    identity.Password.RequireNonAlphanumeric = false;

                    identity.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    identity.Lockout.MaxFailedAccessAttempts = 5;
                    identity.Lockout.AllowedForNewUsers = true;

                    identity.SignIn.RequireConfirmedAccount = false;
                },
            };

            builder.Services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfig)
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddDefaultTokenProviders();
        }
    }
}
