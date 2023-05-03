using BlazorApp1.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MudBlazor.Charts;
using NPOI.SS.Formula.Functions;
using System.Threading.Channels;

namespace BlazorApp1.Services
{
    public interface IModel
    {
        string Id { get; set; }
    }
    public interface IHistoryModel<T> : IModel
    {
        List<string> FindChanges(T newObject);
    }

    public abstract class AbstractHistory 
    {
        public string? Creator { get; set; } = null;
        public DateTime? CreateTime { get; set; } = null;
        public string? UserLastUpdate { get; set; } = null;
        public DateTime? LastUpdateTime { get; set; } = null;

    }

    public class BaseService<T> where T : IModel
    {
        public MongoClient _mongoClient { get; set; } = null;
        public IMongoDatabase _database { get; set; } = null;
        public IMongoCollection<T> _table { get; set; } = null;

        public BaseService(string collectionName) 
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string connectionString = configuration.GetValue<string>("ConnectionStrings:MongoContext");
            string database = configuration.GetValue<string>("ConnectionStrings:MongoDB");

            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase(database);
            _table = _database.GetCollection<T>(collectionName);
        }
        public virtual long Delete(string objectId)
        {
            DeleteResult result = _table.DeleteOne(x => x.Id == objectId);
            return result.DeletedCount;
        }
        public virtual T GetOne(string objectId)
        {
            return _table.Find(x => x.Id == objectId).FirstOrDefault();
        }

        public virtual List<T> GetAll()
        {
            return _table.Find(FilterDefinition<T>.Empty).ToList();
        }

        public virtual void SaveOrUpdate(T myobject)
        {
            var Obj = _table.Find(x => x.Id == myobject.Id).FirstOrDefault();
            if (Obj == null)
            {
                _table.InsertOne(myobject);
            }
            else
            {
                _table.ReplaceOne(x => x.Id == myobject.Id, myobject);
            }
        }
    }






    public class HistoryModel<T>
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public T HistoryObject { get; set; }
        public HistoryModel(T obj)
        {
            this.HistoryObject = obj;
        }

        public List<string>? Changes { get; set; } = null;
        public string ChangesToString() 
        {
            string result = string.Empty;
            if(this.Changes != null && this.Changes.Count() > 0) 
            {
                foreach (var change in this.Changes)
                {
                    result += change + " ";
                }
            }
            else 
            {
                result += "Без изменений";
            }
            return result;
        }
    }

    public class ServiceWithHistory<T1, T2> : BaseService<T1> 
        where T1 : IHistoryModel<T1>
        where T2 : HistoryModel<T1>
    {
        public IMongoCollection<T2> _historyTable { get; set; } = null;
        public ServiceWithHistory(string collectionName, string collectionHistoryName) : base(collectionName)
        {
            _historyTable = _database.GetCollection<T2>(collectionHistoryName);
        }
        public override long Delete(string objectId)
        {
            //SaveHistory
            try
            {
                T1? objectToSave = base.GetOne(objectId);
                var historyObject = new HistoryModel<T1>(objectToSave);
                historyObject.Changes = new List<string> { "Delete" };
                _historyTable.InsertOne((T2)historyObject);
            }
            catch(Exception ex)
            {
                Console.WriteLine("ServiceWithHistory_Delete()_HistoryException: " + ex.Message);
            }
            

            return base.Delete(objectId);
        }
        public override void SaveOrUpdate(T1 myobject)
        {
            //SaveHistory
            try
            {
                var historyObject = new HistoryModel<T1>(myobject);

                var lastObject = _historyTable.Find(x => x.HistoryObject.Id == myobject.Id).SortByDescending(x => x.Id).FirstOrDefault();
                if (lastObject!=null)
                {
                    var changes = historyObject.HistoryObject.FindChanges(lastObject.HistoryObject);
                    if (changes != null && changes.Count > 0)
                        historyObject.Changes = changes;
                }
                else
                {
                    historyObject.Changes = new List<string>() { "New" };
                }
                    
                _historyTable.InsertOne((T2)historyObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ServiceWithHistory_SaveOrUpdate()_HistoryException: " + ex.Message);
            }

            base.SaveOrUpdate(myobject);
        }
        public void SaveOrUpdateWithoutHistory(T1 myobject) 
        {
            base.SaveOrUpdate(myobject);
        }

        public virtual List<T2> GetTopHistory(string inputId)
        {
            //return _historyTable.Find(x=>x.HistoryObject.Id == inputId).ToList();

            return _historyTable.Find(x => x.HistoryObject.Id == inputId).Sort(Builders<T2>.Sort.Descending("_id")).Limit(20).ToList();
        }

        public virtual List<T2> GetAllHistory(string inputId)
        {
            return _historyTable.Find(x=>x.HistoryObject.Id == inputId).Sort(Builders<T2>.Sort.Descending("_id")).ToList();
        }
    }
}
