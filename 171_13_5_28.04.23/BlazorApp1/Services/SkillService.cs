using BlazorApp1.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NPOI.SS.Formula.Functions;
using System.Linq;

namespace BlazorApp1.Services
{
    public class SkillService : BaseService<Skill>
    {
        public SkillService(string collectionName) : base(collectionName)
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
            var field = new StringFieldDefinition<Skill>("SkillName");
            var indexDefinition = new IndexKeysDefinitionBuilder<Skill>().Ascending(field);
            
            _table.Indexes.CreateOne(indexDefinition, options);
        }

        //Этот метод получает коллекцию объектов и находит объекты, которые уже есть в базе данных.Затем он создает две коллекции: новые объекты и обновляемые объекты.Новые объекты записываются с помощью метода InsertMany, а обновляемые объекты записываются с помощью метода BulkWrite.
        public void SaveOrUpdateMany(IEnumerable<Skill> objects)
        {
            var deletedobjectIds = objects.Where(x => x.Selector == "deleted").Select(x => x.Id);
            // Удаляем объекты с Selector == "deleted"
            if (deletedobjectIds.Any())
            {
                var deleteFilter = Builders<Skill>.Filter.In(x => x.Id, deletedobjectIds);
                _table.DeleteMany(deleteFilter);
            }

            //var objectIds = objects.Where(x=>x.Selector != "deleted").Select(x => x.Id);
            var inputObjects = objects.Where(x => x.Selector != "deleted").ToList();
            var baseObjects = base.GetAll();

            var updatedObjects = baseObjects.Where(x => inputObjects.Select(x => x.Id).Contains(x.Id));
            if (updatedObjects.Any())
            {
                var bulkWriteRequests = updatedObjects.Select(x => new ReplaceOneModel<Skill>(Builders<Skill>.Filter.Eq(y => y.Id, x.Id), x));
                _table.BulkWrite(bulkWriteRequests);
            }

            var newObjects = inputObjects.Except(updatedObjects);
            if (newObjects.Any())
            {
                _table.InsertMany(newObjects);
            }
        }
    }
}
