using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models
{
    public class Brigade
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string BrigadeName { get; set; }
        public string BrigadeDescription { get; set; }

        public string Brigadier { get; set; } //ID (Бригадир может не входить в состав)
        public List<string> Members { get; set; } //id_участников
        public List<string> Skills { get; set; }  //МожноВзять Из Участников или Задать Отдельно(даже если у участников нет этих скилов)
    }

    public class BrigadeFull : Brigade
    {
        public Employee Brigadier { get; set; }
        public List<Employee> Members { get; set; }
        public List<Skill> Skills { get; set; }
    }
}
