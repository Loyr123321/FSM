using BlazorApp1.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    public class UserService : BaseService<User>
    {
        public UserService(string collectionName) : base(collectionName)
        {
            //unique
            var options = new CreateIndexOptions()
            {
                Unique = true,

                Collation = new Collation("en",
                strength: CollationStrength.Primary,
                numericOrdering: false,
                caseLevel: false,
                backwards: false,
                normalization: true) //Не учитывать регистр
            };
            var field = new StringFieldDefinition<User>("Email");
            var indexDefinition = new IndexKeysDefinitionBuilder<User>().Ascending(field);

            _table.Indexes.CreateOne(indexDefinition, options);
        }

        public User GetUserByEmail(string email)
        {
            return _table.Find(x => x.Email == email).FirstOrDefault();
        }
    }
}
