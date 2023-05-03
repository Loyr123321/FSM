using BlazorApp1.Services;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Policy;

namespace BlazorApp1.Models
{
    public class User : BaseUser, IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [BsonIgnore]
        public string ConfirmPassword { get; set; }
        public List<string>? Roles { get; set;}

        //Id основного формата таблицы заказов
        //public string? MainUserGridSettingsId { get; set; } = null;
        public string? MyCompany { get; set; }

        public User() { }
        public static User CloneUserWithoutPass(User inputUser) 
        {
            User newUser = new();
            newUser.Id = inputUser.Id;
            newUser.FirstName = inputUser.FirstName;
            newUser.LastName = inputUser.LastName;
            newUser.MiddleName = inputUser.MiddleName;
            newUser.Email = inputUser.Email;
            newUser.Roles = inputUser.Roles;
            newUser.MyCompany= inputUser.MyCompany;

            return newUser;
        }

        public string GetFullName()
        {
            return this.LastName + " " + this.FirstName + " " + this.MiddleName;
        }
    }
}
