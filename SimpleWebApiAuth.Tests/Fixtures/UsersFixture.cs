using SimpleWebApiAuth.Application.DTOs.Authentication.Requests;
using SimpleWebApiAuth.Application.DTOs.Authentication.Responses;
using SimpleWebApiAuth.Domain.Identity;
using System.Security.Claims;

namespace SimpleWebApiAuth.Tests.Fixtures
{
    public static class UsersFixture
    {
        public static LoginRequest GetLoginRequest(RegisterRequest request)
            => new()
            {
                Email = request.Email,
                Password = request.Password,
            };

        public static RegisterRequest GetRegisterRequest()
            => new()
            {
                Email = "test@test.dev",
                Password = "123456",
                ConfirmPassword = "123456",
                FullName = "Renan Fernandes",
                Username = "renan",
            };

        public static LoginResponse GetLoginResponse(RegisterRequest request)
            => new()
            {
                AccessToken = "token",
                Claims =
                [
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim(ClaimTypes.Role, "common"),
                ],
                Email = request.Email,
                Roles =
                [
                    "admin",
                    "common",
                ],
                UserName = request.Username,
            };

        public static IEnumerable<ApplicationUser> GetListOfUsers()
            => [
                    new ApplicationUser
                    {
                        Email = "test@test.dev",
                        FullName = "Test",
                        UserName = "test",
                    },
                    new ApplicationUser
                    {
                        Email = "test2@test.dev",
                        FullName = "Test Test",
                        UserName = "test2",
                    },
                    new ApplicationUser
                    {
                        Email = "test3@test.dev",
                        FullName = "Test Test",
                        UserName = "test3",
                    },
                ];
    }
}
