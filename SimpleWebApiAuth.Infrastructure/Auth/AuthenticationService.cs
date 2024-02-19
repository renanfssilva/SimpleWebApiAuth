using Microsoft.AspNetCore.Identity;
using SimpleWebApiAuth.Application;
using SimpleWebApiAuth.Application.DTOs.Authentication.Requests;
using SimpleWebApiAuth.Application.DTOs.Authentication.Responses;
using SimpleWebApiAuth.Application.Exceptions;
using SimpleWebApiAuth.Application.Services.Auth;
using SimpleWebApiAuth.Domain.Identity;

namespace SimpleWebApiAuth.Infrastructure.Auth
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task Register(RegisterRequest request);
        Task<string> RegisterAdmin(string? username);
    }

    public class AuthenticationService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ITokenService tokenService) : IAuthenticationService
    {
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email) ?? throw new UserNotFoundException("Invalid email or password");
            if (!await userManager.CheckPasswordAsync(user, request.Password))
                throw new UserNotFoundException("Invalid email or password");

            var token = tokenService.GenerateToken(user);

            return new LoginResponse
            {
                AccessToken = token,
                Email = user?.Email!,
                UserName = user?.UserName!,
                Roles = await userManager.GetRolesAsync(user!),
                Claims = await userManager.GetClaimsAsync(user!),
            };
        }

        public async Task Register(RegisterRequest request)
        {
            var userExists = await userManager.FindByEmailAsync(request.Email);
            if (userExists is not null)
                throw new InvalidOperationException("User already exists");

            var user = new ApplicationUser(request.Username, request.FullName, request.Email);

            var createUserResult = await userManager.CreateAsync(user, request.Password);
            if (!createUserResult.Succeeded)
                throw new InvalidOperationException($"Create user failed. {createUserResult?.Errors?.First()?.Description}");

            var addUserToRoleResult = await AddToRoleAsync(user, Settings.CommonRole);
            if (!addUserToRoleResult.Succeeded)
                throw new InvalidOperationException($"Create user succeeded but could not add user to role {addUserToRoleResult?.Errors?.First()?.Description}");
        }

        public async Task<string> RegisterAdmin(string? username)
        {
            if (string.IsNullOrWhiteSpace(username) || !string.Equals(username, "renan", StringComparison.InvariantCultureIgnoreCase))
                throw new UnauthorizedAccessException("Unable to fulfill the request");

            var user = await userManager.FindByNameAsync(username) ?? throw new UserNotFoundException("User not found");

            var addUserToRoleResult = await AddToRoleAsync(user, Settings.AdminRole);
            if (!addUserToRoleResult.Succeeded)
                throw new InvalidOperationException($"Could not add user to role. {addUserToRoleResult?.Errors?.First()?.Description}");

            return tokenService.GenerateToken(user);
        }

        private async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                role = new ApplicationRole(roleName);
                await roleManager.CreateAsync(role);
            }

            return await userManager.AddToRoleAsync(user, roleName);
        }
    }
}
