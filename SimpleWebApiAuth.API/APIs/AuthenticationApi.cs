using SimpleWebApiAuth.API.Validation;
using SimpleWebApiAuth.Application.DTOs.Authentication.Requests;
using SimpleWebApiAuth.Application.Exceptions;
using SimpleWebApiAuth.Infrastructure.Auth;
using System.Security.Claims;

namespace SimpleWebApiAuth.API.APIs
{
    public static class AuthenticationApi
    {
        public static void ConfigureAuthenticationApi(this WebApplication app)
        {
            var authentication = app.MapGroup("/");

            authentication.MapPost("login", Login)
                .AllowAnonymous()
                .AddEndpointFilter<ValidationFilter<LoginRequest>>();
            authentication.MapPost("signup", Register)
                .AllowAnonymous()
                .AddEndpointFilter<ValidationFilter<RegisterRequest>>();
            authentication.MapPost("admin", RegisterAdmin)
                .RequireAuthorization("User");
        }

        private static async Task<IResult> Register(RegisterRequest request, IAuthenticationService authenticationService)
        {
            try
            {
                await authenticationService.Register(request);
                return TypedResults.Created();
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message);
            }
        }

        private static async Task<IResult> Login(LoginRequest request, IAuthenticationService authenticationService)
        {
            try
            {
                var response = await authenticationService.Login(request);
                return TypedResults.Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                return TypedResults.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message);
            }
        }

        private static async Task<IResult> RegisterAdmin(IAuthenticationService authenticationService, ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                var accessToken = await authenticationService.RegisterAdmin(claimsPrincipal.Identity?.Name);
                return TypedResults.Ok(new
                {
                    accessToken,
                });
            }
            catch (UserNotFoundException ex)
            {
                return TypedResults.NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Forbid();
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message);
            }
        }
    }
}
