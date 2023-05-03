using BlazorApp1.Models;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    public class JwtWhiteListService : BaseService<JwtRow>
    {
        public JwtWhiteListService(string collectionName) : base(collectionName)
        {
            //unique
            var options = new CreateIndexOptions()
            {
                Unique = true,
                Collation = new Collation("en", strength: CollationStrength.Primary) //Не учитывать регистр
            };
            var field = new StringFieldDefinition<JwtRow>("UserId");
            var indexDefinition = new IndexKeysDefinitionBuilder<JwtRow>().Ascending(field);

            _table.Indexes.CreateOne(indexDefinition, options);
        }

        public JwtRow GetJwtRowByUserId(string userId)
        {
            return _table.Find(x => x.UserId == userId).FirstOrDefault();
        }
        public JwtRow GetJwtRowByTokenId(string tokenId)
        {
            return _table.Find(x => x.AccessTokenId == tokenId || x.RefreshTokenId == tokenId).FirstOrDefault();
        }

    }
}
