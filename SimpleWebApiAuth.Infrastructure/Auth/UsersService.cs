using Microsoft.AspNetCore.Identity;
using SimpleWebApiAuth.Application.Exceptions;
using SimpleWebApiAuth.Domain.Identity;

namespace SimpleWebApiAuth.Infrastructure.Auth
{
    public interface IUsersService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
        Task<ApplicationUser> GetByUsername(string username);
    }

    public class UsersService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : IUsersService
    {
        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            var users = new List<ApplicationUser>();

            var roles = roleManager.Roles;
            foreach (var role in roles)
                users.AddRange(await userManager.GetUsersInRoleAsync(role.Name!));

            return users.GroupBy(u => u.Id).Select(g => g.First());
        }

        public async Task<ApplicationUser> GetByUsername(string username)
            => await userManager.FindByNameAsync(username)
                ?? throw new UserNotFoundException("User not found");
    }
}
