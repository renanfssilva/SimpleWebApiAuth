using SimpleWebApiAuth.Infrastructure.Auth;
using System.Security.Claims;

namespace SimpleWebApiAuth.API.APIs
{
    public static class UsersApi
    {
        public static void ConfigureUsersApi(this WebApplication app)
        {
            var users = app.MapGroup("/users");

            users.MapGet("/", Get)
                .RequireAuthorization("Administrator");

            users.MapGet("/current", GetCurrent)
                .RequireAuthorization("User");
        }

        private static async Task<IResult> Get(IUsersService usersService)
        {
            try
            {
                var users = await usersService.GetAllUsers();

                return users.Any()
                    ? TypedResults.Ok(users)
                    : TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message);
            }
        }

        private static async Task<IResult> GetCurrent(IUsersService usersService, ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                var user = await usersService.GetByUsername(claimsPrincipal.Identity!.Name!);
                return user is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(user);
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message);
            }
        }
    }
}
