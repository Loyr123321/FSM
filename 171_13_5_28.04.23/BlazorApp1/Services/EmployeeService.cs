using BlazorApp1.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    public class EmployeeService : ServiceWithHistory<Employee, HistoryModel<Employee>>
    {
        public EmployeeService(string collectionName, string collectionHistoryName) : base(collectionName, collectionHistoryName)
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
            var field0 = new StringFieldDefinition<Employee>("Login");
            var indexDefinition0 = new IndexKeysDefinitionBuilder<Employee>().Ascending(field0);

            var field1 = new StringFieldDefinition<Employee>("Phone");
            var indexDefinition1 = new IndexKeysDefinitionBuilder<Employee>().Ascending(field1);

            _table.Indexes.CreateOne(indexDefinition0, options);
            _table.Indexes.CreateOne(indexDefinition1, options);
        }

        public override long Delete(string objectId)
        {
            ////УдалитьМастера Из Всех Бригад (Скилы не трогать) //Изменить на массовую замену!!!
            //BrigadeService brigadeService = new BrigadeService();
            //var brigades = brigadeService.GetBrigades();
            //foreach (Brigade brigade in brigades)
            //{
            //    brigade.Members.Remove(objectId);
            //    brigadeService.SaveOrUpdate(brigade);
            //}
            ////---------------------------------------------------

            return base.Delete(objectId);
        }

        public Employee GetEmployeeByLogin(string login)
        {
            return _table.Find(x => x.Login == login).FirstOrDefault();
        }
        public Employee GetEmployeeByPhone(string phone)
        {
            return _table.Find(x => x.Phone == phone).FirstOrDefault();
        }

    }
}
