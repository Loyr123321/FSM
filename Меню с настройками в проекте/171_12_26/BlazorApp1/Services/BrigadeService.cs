using BlazorApp1.Models;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    public class BrigadeService
    {
        private MongoClient _mongoClient = null;
        private IMongoDatabase _database = null;
        private IMongoCollection<Brigade> _table = null;
        public BrigadeService()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string connectionString = configuration.GetValue<string>("ConnectionStrings:MongoContext");
            string database = configuration.GetValue<string>("ConnectionStrings:MongoDB");

            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase(database);
            _table = _database.GetCollection<Brigade>("Brigades");
        }
        public long Delete(string objectId)
        {
            DeleteResult result = _table.DeleteOne(x => x.Id == objectId);
            return result.DeletedCount;
        }
        public Brigade GetBrigade(string objectId)
        {
            return _table.Find(x => x.Id == objectId).FirstOrDefault();
        }

        public List<Brigade> GetBrigades()
        {
            return _table.Find(FilterDefinition<Brigade>.Empty).ToList();
        }

        public void SaveOrUpdate(Brigade brigade)
        {
            var Obj = _table.Find(x => x.Id == brigade.Id).FirstOrDefault();
            if (Obj == null)
            {
                _table.InsertOne(brigade);
            }
            else
            {
                _table.ReplaceOne(x => x.Id == brigade.Id, brigade);
            }
        }




    }



}
