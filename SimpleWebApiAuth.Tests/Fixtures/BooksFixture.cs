using SimpleWebApiAuth.Domain.Books;

namespace SimpleWebApiAuth.Tests.Fixtures
{
    public static class BooksFixture
    {
        public static IEnumerable<Book> GetBooksList() =>
            [
                new Book
                {
                    Id = "123456",
                    Author = "Test 1",
                    BookName = "Test 1",
                    Category = "Test 1",
                    Price = 1M,
                },
                new Book{

                    Id = "654321",
                    Author = "Test 2",
                    BookName = "Test 2",
                    Category = "Test 2",
                    Price = 2M,
                },
            ];
    }
}
