using BlazorApp1.Services;
using MongoDB.Bson.Serialization.Attributes;
using NPOI.SS.Formula.Functions;

namespace BlazorApp1.Models
{
    public class ExecutorService : IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        public double OurPercent { get; private set; }
        public double ExecutorPercent { get; private set; }

        public ExecutorService(StoreService service) 
        {

        }

        //Предусмотреть выплату монтажнику и без % 5000
        //диспетчер м редактировать +-


        //public double ExecutorPayment { get; set; }
        //public double CalculatePayment() 
        //{
        //}


    }
}
