using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleWebApiAuth.Domain.Identity;
using SimpleWebApiAuth.Infrastructure.Auth;
using SimpleWebApiAuth.Tests.Fixtures;
using SimpleWebApiAuth.Tests.Helpers;
using System.Net;
using System.Net.Http.Headers;

namespace SimpleWebApiAuth.Tests.APIs
{
    public class UsersApiTests
    {
        private readonly IUsersService _usersService = Substitute.For<IUsersService>();

        #region Successful Tests

        [Fact]
        public async Task GetAllUsers_OnSuccess_ReturnsListOfUsers()
        {
            //Arrange
            var usersListMock = UsersFixture.GetListOfUsers();
            _usersService.GetAllUsers().ReturnsForAnyArgs(Task.FromResult(usersListMock));
            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_usersService);
            });

            var registerRequest = UsersFixture.GetRegisterRequest();
            var applicationUser = new ApplicationUser(registerRequest.Username, registerRequest.FullName, registerRequest.Email);
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"/users");
            var responseText = await response.Content.ReadAsStringAsync();
            var usersList = JsonConvert.DeserializeObject<List<ApplicationUser>>(responseText);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            usersList.Should().NotBeEmpty();
            usersList.Should().HaveCount(usersListMock.Count());
        }

        [Fact]
        public async Task GetCurrentUser_OnSuccess_ReturnsUser()
        {
            //Arrange
            var userMock = UsersFixture.GetListOfUsers().First();
            _usersService.GetByUsername(userMock.UserName!).ReturnsForAnyArgs(Task.FromResult(userMock));
            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_usersService);
            });

            var registerRequest = UsersFixture.GetRegisterRequest();
            var applicationUser = new ApplicationUser(registerRequest.Username, registerRequest.FullName, registerRequest.Email);
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"/users/current");
            var responseText = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<ApplicationUser>(responseText);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            user!.UserName.Should().Be(userMock.UserName);
        }

        #endregion

        #region NotFound Tests

        [Fact]
        public async Task GetAllUsers_ReturnsNotFound_WhenThereIsNoUser()
        {
            //Arrange
            _usersService.GetAllUsers().ReturnsForAnyArgs(new List<ApplicationUser>());
            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_usersService);
            });

            var registerRequest = UsersFixture.GetRegisterRequest();
            var applicationUser = new ApplicationUser(registerRequest.Username, registerRequest.FullName, registerRequest.Email);
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"/users");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsNotFound_WhenUserWasNotFound()
        {
            //Arrange
            _usersService.GetByUsername(string.Empty).ReturnsNullForAnyArgs();
            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_usersService);
            });

            var registerRequest = UsersFixture.GetRegisterRequest();
            var applicationUser = new ApplicationUser(registerRequest.Username, registerRequest.FullName, registerRequest.Email);
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"/users/current");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}
