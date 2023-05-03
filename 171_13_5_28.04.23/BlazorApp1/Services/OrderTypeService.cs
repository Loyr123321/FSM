using BlazorApp1.Models;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    public class OrderTypeService : BaseService<OrderType>
    {
        public OrderTypeService(string collectionName) : base(collectionName)
        {
            //unique
            var options = new CreateIndexOptions() {
                Unique = true,

                Collation = new Collation("en",
                strength: CollationStrength.Primary,
                numericOrdering: false,
                caseLevel: false,
                backwards: false,
                normalization: true) //Не учитывать регистр
            };
            

            var field0 = new StringFieldDefinition<OrderType>("TypeName");
            var indexDefinition0 = new IndexKeysDefinitionBuilder<OrderType>().Ascending(field0);

            var field1 = new StringFieldDefinition<OrderType>("ShortName");
            var indexDefinition1 = new IndexKeysDefinitionBuilder<OrderType>().Ascending(field1);

            _table.Indexes.CreateOne(indexDefinition0, options);
            _table.Indexes.CreateOne(indexDefinition1, options);

        }
    }
}
