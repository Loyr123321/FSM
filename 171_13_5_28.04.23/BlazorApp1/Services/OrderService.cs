using BlazorApp1.Models;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    public class OrderService : ServiceWithHistory<Order, HistoryModel<Order>>
    {
        public OrderService(string collectionName, string collectionHistoryName) : base(collectionName, collectionHistoryName)
        {
        }
        public List<Order> GetOrdersByExecutorId(string executorId)
        {
            return _table.Find(x => x.OrderEmployeeExecutor.Id == executorId).ToList();
        }

        public List<Order> GetOrdersByTemplateId(string template)
        {
            return _table.Find(x => x.Template.Id == template).ToList();
        }

        public void UpdateOrdersTemplate(List<Order> orders)
        {
            //Обновить целиком (Старые данные будут утеряны если поля удалены)
            //Нужно сделать старое остается старым. В старые заказы добавляются новые поля по необходимости
            //https://www.mongodb.com/docs/manual/reference/operator/update/setOnInsert/#up._S_setOnInsert
            if (orders.Any())
            {
                var bulkWriteRequests = orders.Select(x => new ReplaceOneModel<Order>(Builders<Order>.Filter.Eq(y => y.Id, x.Id), x));
                _table.BulkWrite(bulkWriteRequests);
            }
        }

    }
}
