using BlazorApp1.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models
{
    //Планировщик
    //Исполнитель будет заранее присылать даты в которые он может(хочет) работать

    //Продумать замену времени
    public class SchedulerEntry : IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string ExecutorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public bool Validation()
        {
            if (StartTime > EndTime) 
            {
                return false;
            }
            if (StartTime < CreateTime)
            {
                return false;
            }
            return true;
        }
    }
}
