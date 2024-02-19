using FluentAssertions;
using NSubstitute;
using SimpleWebApiAuth.Infrastructure.Auth;
using SimpleWebApiAuth.Tests.Fixtures;
using SimpleWebApiAuth.Tests.Helpers;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Newtonsoft.Json;
using SimpleWebApiAuth.Domain.Identity;
using System.Net.Http.Headers;
using System.Dynamic;
using NSubstitute.ExceptionExtensions;
using SimpleWebApiAuth.Application.Exceptions;

namespace SimpleWebApiAuth.Tests.APIs
{
    public class AuthenticationApiTests
    {
        private readonly IAuthenticationService _authenticationService = Substitute.For<IAuthenticationService>();

        #region Successful Tests

        [Fact]
        public async Task Signup_OnSuccess_ReturnsCreated()
        {
            //Arrange
            var registerRequest = UsersFixture.GetRegisterRequest();
            _authenticationService.Register(registerRequest).ReturnsForAnyArgs(Task.CompletedTask);

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_authenticationService);
            });

            var httpClient = app.CreateClient();
            var registerContent = new StringContent(JsonConvert.SerializeObject(registerRequest), Encoding.UTF8, "application/json");

            //Act
            var response = await httpClient.PostAsync($"/signup", registerContent);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Login_OnSuccess_ReturnsOK()
        {
            //Arrange
            var registerRequest = UsersFixture.GetRegisterRequest();
            var loginRequest = UsersFixture.GetLoginRequest(registerRequest);
            var loginResponse = UsersFixture.GetLoginResponse(registerRequest);
            _authenticationService.Login(loginRequest).ReturnsForAnyArgs(Task.FromResult(loginResponse));

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_authenticationService);
            });

            var httpClient = app.CreateClient();
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            //Act
            var response = await httpClient.PostAsync($"/login", loginContent);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterAdmin_OnSuccess_ReturnsAccessToken()
        {
            //Arrange
            var registerRequest = UsersFixture.GetRegisterRequest();

            var applicationUser = new ApplicationUser(registerRequest.Username, registerRequest.FullName, registerRequest.Email);
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            _authenticationService.RegisterAdmin(registerRequest.Username).ReturnsForAnyArgs(Task.FromResult(accessToken));

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_authenticationService);
            });
            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.PostAsync($"/admin", default);
            var responseText = await response.Content.ReadAsStringAsync();
            var adminResult = JsonConvert.DeserializeObject<ExpandoObject>(responseText);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            adminResult!.First(x => x.Key.Equals("accessToken", StringComparison.InvariantCultureIgnoreCase)).Value.Should().Be(accessToken);
        }

        #endregion

        #region NotFound Tests

        [Fact]
        public async Task Login_ReturnsNotFound_WhenUserDoesNotExists()
        {
            //Arrange
            var registerRequest = UsersFixture.GetRegisterRequest();
            var loginRequest = UsersFixture.GetLoginRequest(registerRequest);
            _authenticationService.Login(loginRequest).ThrowsForAnyArgs(new UserNotFoundException("Invalid email or password"));

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_authenticationService);
            });

            var httpClient = app.CreateClient();
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            //Act
            var response = await httpClient.PostAsync($"/login", loginContent);
            var responseText = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseText.Should().Be("\"Invalid email or password\"");
        }

        [Fact]
        public async Task RegisterAdmin_ReturnsNotFound_WhenUserDoesNotExists()
        {
            //Arrange
            _authenticationService.RegisterAdmin(default).ThrowsForAnyArgs(new UserNotFoundException("User not found"));
            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_authenticationService);
            });

            var registerRequest = UsersFixture.GetRegisterRequest();
            var applicationUser = new ApplicationUser(registerRequest.Username, registerRequest.FullName, registerRequest.Email);
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.PostAsync($"/admin", default);
            var responseText = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseText.Should().Be("\"User not found\"");
        }

        #endregion

        #region Forbidden Tests

        [Fact]
        public async Task RegisterAdmin_ReturnsForbid_WhenUserIsNotTheOneSpecified()
        {
            //Arrange
            _authenticationService.RegisterAdmin(default).ThrowsForAnyArgs(new UnauthorizedAccessException("Unable to fulfill the request"));
            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_authenticationService);
            });

            var registerRequest = UsersFixture.GetRegisterRequest();
            var applicationUser = new ApplicationUser(registerRequest.Username, registerRequest.FullName, registerRequest.Email);
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.PostAsync($"/admin", default);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion
    }
}
