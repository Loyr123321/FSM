using BlazorApp1.Services;
using MongoDB.Bson.Serialization.Attributes;
using static MudBlazor.CategoryTypes;

namespace BlazorApp1.Models
{
    public class ReassignmentReason : IModel //Причина переназначения монтажника
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        public string OrderId { get; set; }

        public string ReasonValue { get; set; } = string.Empty;

        public Employee Executor { get; set; }
        //public Employee NewExecutor { get; set; }
        public bool IsRatingDowngrade { get; set; }
    }
}
