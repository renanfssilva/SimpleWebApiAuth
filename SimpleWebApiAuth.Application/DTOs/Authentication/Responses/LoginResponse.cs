using System.Security.Claims;

namespace SimpleWebApiAuth.Application.DTOs.Authentication.Responses
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = [];
        public IEnumerable<Claim> Claims { get; set; } = [];
    }
}
