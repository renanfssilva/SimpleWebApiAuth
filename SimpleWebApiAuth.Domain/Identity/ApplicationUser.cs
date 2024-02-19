using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace SimpleWebApiAuth.Domain.Identity
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public ApplicationUser() : base()
        {
        }

        public ApplicationUser(string userName, string fullName, string email) : base(userName, email)
        {
            UserName = userName;
            FullName = fullName;
            Email = email;
        }
    }
}
