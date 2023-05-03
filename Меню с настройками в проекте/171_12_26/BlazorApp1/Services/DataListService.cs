using BlazorApp1.Models;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace BlazorApp1.Services
{
    public class DataListService : BaseService<DataList>
    {
        public DataListService(string collectionName) : base(collectionName)
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
            var field = new StringFieldDefinition<DataList>("Name");
            var indexDefinition = new IndexKeysDefinitionBuilder<DataList>().Ascending(field);

            _table.Indexes.CreateOne(indexDefinition, options);
        }
    }
}
