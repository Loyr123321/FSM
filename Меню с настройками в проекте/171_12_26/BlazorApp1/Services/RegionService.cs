using BlazorApp1.Models;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    public class RegionService : BaseService<Region>
    {
        public RegionService(string collectionName) : base(collectionName)
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
            var field = new StringFieldDefinition<Region>("RegionName");
            var indexDefinition = new IndexKeysDefinitionBuilder<Region>().Ascending(field);

            _table.Indexes.CreateOne(indexDefinition, options);
        }
    }
}
