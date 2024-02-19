using SimpleWebApiAuth.API.Validation;
using SimpleWebApiAuth.Domain.Books;
using SimpleWebApiAuth.Infrastructure;

namespace SimpleWebApiAuth.API.APIs
{
    public static class BooksApi
    {
        public static void ConfigureBooksApi(this WebApplication app)
        {
            var users = app.MapGroup("/books");

            users.MapGet("/", Get)
                .RequireAuthorization("User");
            users.MapGet("/{id}", GetById)
                .RequireAuthorization("User");
            users.MapPost("/", Post)
                .RequireAuthorization("Administrator")
                .AddEndpointFilter<ValidationFilter<Book>>();
            users.MapPut("/{id}", Update)
                .RequireAuthorization("Administrator")
                .AddEndpointFilter<ValidationFilter<Book>>();
            users.MapDelete("/{id}", Delete)
                .RequireAuthorization("Administrator");
        }

        private static async Task<IResult> Get(ICrudService<Book> booksService)
        {
            try
            {
                var books = await booksService.GetAsync();

                return books.Any()
                    ? TypedResults.Ok(books)
                    : TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message);
            }
        }

        private static async Task<IResult> GetById(string id, ICrudService<Book> booksService)
        {
            try
            {
                var book = await booksService.GetAsync(id);
                return book == null ? TypedResults.NotFound() : TypedResults.Ok(book);
            }
            catch (Exception e)
            {
                return TypedResults.Problem(e.Message);
            }
        }

        private static async Task<IResult> Post(Book newBook, ICrudService<Book> booksService)
        {
            try
            {
                await booksService.CreateAsync(newBook);
                return TypedResults.Created($"/books/{newBook.Id}", newBook);
            }
            catch (Exception e)
            {
                return TypedResults.Problem(e.Message);
            }
        }

        private static async Task<IResult> Update(string id, Book updatedBook, ICrudService<Book> booksService)
        {
            try
            {
                var existingBook = await booksService.GetAsync(id);

                if (existingBook is null)
                    return TypedResults.NotFound();

                updatedBook.Id = existingBook.Id;

                await booksService.UpdateAsync(id, updatedBook);

                return TypedResults.NoContent();
            }
            catch (Exception e)
            {
                return TypedResults.Problem(e.Message);
            }
        }

        private static async Task<IResult> Delete(string id, ICrudService<Book> booksService)
        {
            try
            {
                if (await booksService.GetAsync(id) is null)
                    return TypedResults.NotFound();

                await booksService.RemoveAsync(id);
                return TypedResults.NoContent();
            }
            catch (Exception e)
            {
                return TypedResults.Problem(e.Message);
            }
        }
    }
}
