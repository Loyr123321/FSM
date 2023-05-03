using BlazorApp1.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models
{
    public class Skill : IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        //длядропконтейнера
        public int Position { get; set; }
        public string Selector { get; set; } = "myselector";
        public string SkillName { get; set; } = string.Empty;
        public override string ToString()
        {
            if (this.SkillName == null)
            {
                return string.Empty;
            }
            return SkillName.ToString();
        }
        public override bool Equals(object? obj)
        {
            var item = obj as Skill;
            if (item == null)
            {
                return false;
            }

            return this.SkillName.Equals(item.SkillName);
        }
        public override int GetHashCode()
        {
            //HashSet использует GetHashCode
            //list.Contains использует Equals а не GetHashCode
            return this.Id.GetHashCode();
        }

    }
}
