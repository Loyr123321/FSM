using BlazorApp1.Models;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    public class OrderTemplateService : ServiceWithHistory<OrderTemplate, HistoryModel<OrderTemplate>>
    {
        public OrderTemplateService(string collectionName, string collectionHistoryName) : base(collectionName, collectionHistoryName)
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
            var field = new StringFieldDefinition<OrderTemplate>("OrderTemplateName");
            var indexDefinition = new IndexKeysDefinitionBuilder<OrderTemplate>().Ascending(field);
            _table.Indexes.CreateOne(indexDefinition, options);
        }
    }
}
