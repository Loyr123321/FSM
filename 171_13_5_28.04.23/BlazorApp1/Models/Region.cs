using BlazorApp1.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models
{
    public class Region : IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string RegionName { get; set; } //Н-р Город
        public List<RegionValue> Values { get; set; }
    }

    public class RegionValue
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]

        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string RegionId { get; set; }
        public string RegionName { get; set; }

        //длядропконтейнера
        public int Position { get; set; }

        public string Selector { get; set; } = "myselector";

        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            if (this.Value == null)
            {
                return string.Empty;
            }
            return Value.ToString();
        }
        public override bool Equals(object? obj)
        {
            var item = obj as RegionValue;
            if (item == null)
            {
                return false;
            }

            return this.Value.Equals(item.Value);
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

    }
}
