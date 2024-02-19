using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace SimpleWebApiAuth.Domain.Identity
{
    [CollectionName("Roles")]
    public class ApplicationRole : MongoIdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }
    }
}
