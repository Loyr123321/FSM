using BlazorApp1.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models
{
    public class JwtRow : IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string AccessTokenId { get; set; }
        public string RefreshTokenId { get; set; }
        public DateTime LastLoginTime { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        public string? UserType { get; set; } = null;
    }
}
