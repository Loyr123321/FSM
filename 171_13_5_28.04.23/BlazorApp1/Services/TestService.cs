using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NuGet.Packaging.Signing;

namespace BlazorApp1.Services
{
    public class Test : IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        public DateTime? DT
        {
            get
            {
                return Date + Time;
            }
            set
            {
                if (value.HasValue)
                {
                    Date = value.Value.Date;
                    Time = value.Value.TimeOfDay;
                }
                else
                {
                    Date = null;
                    Time = null;
                }
            }
        }
        [BsonIgnore]
        public DateTime? Date { get; set; }
        [BsonIgnore]
        public TimeSpan? Time { get; set; }

    }

    public class TestService : BaseService<Test>
    {
        public TestService(string collectionName) : base(collectionName)
        {
        }
    }
}
