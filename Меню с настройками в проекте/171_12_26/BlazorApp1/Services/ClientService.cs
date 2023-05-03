using BlazorApp1.Models;
using BlazorApp1.Services;
using MongoDB.Driver;
using System.Security.AccessControl;

namespace BlazorApp1.Services
{  
    public class ClientService : ServiceWithHistory<Client, HistoryModel<Client>>
    {
        public ClientService(string collectionName, string collectionHistoryName) : base(collectionName, collectionHistoryName)
        {
            //unique
            var options = new CreateIndexOptions()
            {
                Unique = true,
                Sparse = true, //Игнорировать дубликаты с null
                Collation = new Collation("en",
                strength: CollationStrength.Primary,
                numericOrdering: false,
                caseLevel: false,
                backwards: false,
                normalization: true) //Не учитывать регистр
            };
            //var field0 = new StringFieldDefinition<Client>("Login");
            //var indexDefinition0 = new IndexKeysDefinitionBuilder<Client>().Ascending(field0);

            var field1 = new StringFieldDefinition<Client>("Phone");
            var indexDefinition1 = new IndexKeysDefinitionBuilder<Client>().Ascending(field1);

            //_table.Indexes.CreateOne(indexDefinition0, options);
            _table.Indexes.CreateOne(indexDefinition1, options);
        }

        public Client GetPhysicalClientByPhone(string phone)
        {
            return _table.Find(x => x.ClientPhone == phone).FirstOrDefault();
        }
        public Client GetLegalClientByName(string name)
        {
            return _table.Find(x => x.OrganizationName == name).FirstOrDefault();
        }
    }
}
