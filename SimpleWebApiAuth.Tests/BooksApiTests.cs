using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleWebApiAuth.Domain.Books;
using SimpleWebApiAuth.Domain.Identity;
using SimpleWebApiAuth.Infrastructure;
using SimpleWebApiAuth.Tests.Fixtures;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SimpleWebApiAuth.Tests
{
    public class BooksApiTests
    {
        private readonly ICrudService<Book> _booksService = Substitute.For<ICrudService<Book>>();

        #region Unauthorized Tests

        [Fact]
        public async Task GetBooks_ReturnUnauthorized_WhenCallerIsNotAuthorized()
        {
            //Arrange
            using var app = new WebApplicationFactory<Program>();
            var httpClient = app.CreateClient();

            //Act
            var response = await httpClient.GetAsync($"/books");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetBookById_ReturnUnauthorized_WhenCallerIsNotAuthorized()
        {
            //Arrange
            using var app = new WebApplicationFactory<Program>();
            var httpClient = app.CreateClient();

            //Act
            var response = await httpClient.GetAsync($"/books/123456");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PostBook_ReturnUnauthorized_WhenCallerIsNotAuthorized()
        {
            //Arrange
            using var app = new WebApplicationFactory<Program>();
            var httpClient = app.CreateClient();

            //Act
            var response = await httpClient.PostAsync($"/books", new StringContent("test", Encoding.UTF8, "application/json"));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PutBook_ReturnUnauthorized_WhenCallerIsNotAuthorized()
        {
            //Arrange
            using var app = new WebApplicationFactory<Program>();
            var httpClient = app.CreateClient();

            //Act
            var response = await httpClient.PutAsync($"/books/123456", new StringContent("test", Encoding.UTF8, "application/json"));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteBook_ReturnUnauthorized_WhenCallerIsNotAuthorized()
        {
            //Arrange
            using var app = new WebApplicationFactory<Program>();
            var httpClient = app.CreateClient();

            //Act
            var response = await httpClient.DeleteAsync($"/books/123456");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region Successful Tests

        [Fact]
        public async Task GetBooks_ReturnBooksList_WhenBooksExist()
        {
            //Arrange
            var books = BooksFixture.GetBooksList();
            _booksService.GetAsync().ReturnsForAnyArgs(books);

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_booksService);
            });

            var applicationUser = new ApplicationUser("renan", "Renan Fernandes", "renanfssilva@gmail.com");
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"/books");
            var responseText = await response.Content.ReadAsStringAsync();
            var bookResult = JsonConvert.DeserializeObject<List<Book>>(responseText);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            bookResult.Should().BeEquivalentTo(books);
        }

        [Fact]
        public async Task GetBookById_ReturnBook_WhenBookExists()
        {
            //Arrange
            var book = BooksFixture.GetBooksList().First();
            _booksService.GetAsync(Arg.Any<string>()).ReturnsForAnyArgs(book);

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_booksService);
            });

            var applicationUser = new ApplicationUser("renan", "Renan Fernandes", "renanfssilva@gmail.com");
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"/books/{book.Id}");
            var responseText = await response.Content.ReadAsStringAsync();
            var bookResult = JsonConvert.DeserializeObject<Book>(responseText);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            bookResult.Should().BeEquivalentTo(book);
        }

        [Fact]
        public async Task PostBook_OnSuccess_ReturnsCreated()
        {
            //Arrange
            var book = BooksFixture.GetBooksList().First();
            _booksService.CreateAsync(Arg.Any<Book>()).ReturnsForAnyArgs(Task.CompletedTask);

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_booksService);
            });

            var applicationUser = new ApplicationUser("renan", "Renan Fernandes", "renanfssilva@gmail.com");
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var bookContent = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");

            //Act
            var response = await httpClient.PostAsync($"/books", bookContent);
            var responseText = await response.Content.ReadAsStringAsync();
            var bookResult = JsonConvert.DeserializeObject<Book>(responseText);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            bookResult.Should().BeEquivalentTo(book);
        }

        [Fact]
        public async Task PutBook_OnSuccess_ReturnsNoContent()
        {
            //Arrange
            var existingBook = BooksFixture.GetBooksList().First();
            var updatedBook = BooksFixture.GetBooksList().Last();
            _booksService.GetAsync(Arg.Any<string>()).ReturnsForAnyArgs(existingBook);
            _booksService.UpdateAsync(Arg.Any<string>(), Arg.Any<Book>()).ReturnsForAnyArgs(Task.CompletedTask);

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_booksService);
            });

            var applicationUser = new ApplicationUser("renan", "Renan Fernandes", "renanfssilva@gmail.com");
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var bookContent = new StringContent(JsonConvert.SerializeObject(updatedBook), Encoding.UTF8, "application/json");

            //Act
            var response = await httpClient.PutAsync($"/books/{existingBook.Id}", bookContent);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteBook_OnSuccess_ReturnsNoContent()
        {
            //Arrange
            var book = BooksFixture.GetBooksList().First();
            _booksService.GetAsync(Arg.Any<string>()).ReturnsForAnyArgs(book);
            _booksService.RemoveAsync(Arg.Any<string>()).ReturnsForAnyArgs(Task.CompletedTask);

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_booksService);
            });

            var applicationUser = new ApplicationUser("renan", "Renan Fernandes", "renanfssilva@gmail.com");
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.DeleteAsync($"/books/{book.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        #endregion

        #region NotFound Tests

        [Fact]
        public async Task GetBooks_ReturnsNotFound_WhenBooksDoesNotExist()
        {
            //Arrange
            _booksService.GetAsync().ReturnsForAnyArgs(new List<Book>());

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_booksService);
            });

            var applicationUser = new ApplicationUser("renan", "Renan Fernandes", "renanfssilva@gmail.com");
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"/books");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetBookById_ReturnsNotFound_WhenBooksDoNotExist()
        {
            //Arrange
            var book = BooksFixture.GetBooksList().First();
            _booksService.GetAsync(Arg.Any<string>()).ReturnsNullForAnyArgs();

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_booksService);
            });

            var applicationUser = new ApplicationUser("renan", "Renan Fernandes", "renanfssilva@gmail.com");
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.GetAsync($"/books/{book.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PutBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            //Arrange
            var existingBook = BooksFixture.GetBooksList().First();
            var updatedBook = BooksFixture.GetBooksList().Last();
            _booksService.GetAsync(Arg.Any<string>()).ReturnsNullForAnyArgs();
            _booksService.UpdateAsync(Arg.Any<string>(), Arg.Any<Book>()).ReturnsForAnyArgs(Task.CompletedTask);

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_booksService);
            });

            var applicationUser = new ApplicationUser("renan", "Renan Fernandes", "renanfssilva@gmail.com");
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var bookContent = new StringContent(JsonConvert.SerializeObject(updatedBook), Encoding.UTF8, "application/json");

            //Act
            var response = await httpClient.PutAsync($"/books/{existingBook.Id}", bookContent);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            //Arrange
            var book = BooksFixture.GetBooksList().First();
            _booksService.GetAsync(Arg.Any<string>()).ReturnsNullForAnyArgs();
            _booksService.RemoveAsync(Arg.Any<string>()).ReturnsForAnyArgs(Task.CompletedTask);

            using var app = new TestWebApplicationFactory(x =>
            {
                x.AddSingleton(_booksService);
            });

            var applicationUser = new ApplicationUser("renan", "Renan Fernandes", "renanfssilva@gmail.com");
            var accessToken = TokenHelper.GenerateToken(applicationUser);

            var httpClient = app.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await httpClient.DeleteAsync($"/books/{book.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}
