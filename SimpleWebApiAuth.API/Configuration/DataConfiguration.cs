using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleWebApiAuth.Application;
using SimpleWebApiAuth.Domain.Books;
using SimpleWebApiAuth.Infrastructure;
using SimpleWebApiAuth.Infrastructure.Data;

namespace SimpleWebApiAuth.API.Configuration
{
    public static class DataConfiguration
    {
        public static void ConfigureData(this WebApplicationBuilder builder)
        {

            builder.Services.Configure<BookStoreDatabaseSettings>(builder.Configuration.GetSection("BookStoreDatabase"));

            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var bookStoreDatabaseSettings = sp.GetService<IOptions<BookStoreDatabaseSettings>>()!.Value;
                return new MongoClient(bookStoreDatabaseSettings.ConnectionString);
            });

            builder.Services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var bookStoreDatabaseSettings = sp.GetService<IOptions<BookStoreDatabaseSettings>>()!.Value;
                return client.GetDatabase(bookStoreDatabaseSettings.DatabaseName);
            });

            builder.Services.AddTransient<ICrudService<Book>, BooksService>();
        }
    }
}
