using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace SimpleWebApiAuth.Domain.Books
{
    [CollectionName("Books")]
    public class Book
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [BsonElement("Name")]
        public string BookName { get; set; } = null!;

        public decimal Price { get; set; }

        public string Category { get; set; } = null!;

        public string Author { get; set; } = null!;
    }
}
