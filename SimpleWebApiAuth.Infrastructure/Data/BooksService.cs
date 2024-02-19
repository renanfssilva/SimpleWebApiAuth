﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleWebApiAuth.Application;
using SimpleWebApiAuth.Domain.Books;

namespace SimpleWebApiAuth.Infrastructure.Data
{
    public class BooksService(IMongoDatabase mongoDatabase, IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings) : ICrudService<Book>
    {
        private readonly IMongoCollection<Book> _booksCollection =
            mongoDatabase.GetCollection<Book>(bookStoreDatabaseSettings.Value.BooksCollectionName);

        public async Task<IEnumerable<Book>> GetAsync() =>
            await _booksCollection.Find(_ => true).ToListAsync();

        public async Task<Book?> GetAsync(string id) =>
            await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Book newBook) =>
            await _booksCollection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, Book updatedBook) =>
            await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _booksCollection.DeleteOneAsync(x => x.Id == id);
    }
}
