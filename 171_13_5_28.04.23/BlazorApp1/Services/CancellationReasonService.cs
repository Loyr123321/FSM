using BlazorApp1.Models;
using MongoDB.Driver;

namespace BlazorApp1.Services
{
    public class CancellationReasonService : BaseService<CancellationReason>
    {
        public CancellationReasonService(string collectionName) : base(collectionName)
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
            var field = new StringFieldDefinition<CancellationReason>("ReasonName");
            var indexDefinition = new IndexKeysDefinitionBuilder<CancellationReason>().Ascending(field);

            _table.Indexes.CreateOne(indexDefinition, options);
        }

        public void CreateDefaultRecords()
        {
            CancellationReason other = new CancellationReason();
            other.Position = -1;
            other.ReasonTechnicalName = "Other";
            other.ReasonName = "Другое";
            other.IsEditAllowed = false;
            SaveOrUpdate(other);

            //Причины для интеграции с руки.ру
            CancellationReason clientCanceledOrder = new CancellationReason();
            clientCanceledOrder.Position = -1;
            clientCanceledOrder.ReasonTechnicalName = "ClientCanceledOrder";
            clientCanceledOrder.ReasonName = "Клиент отказался от заказа";
            clientCanceledOrder.IsEditAllowed = false;
            SaveOrUpdate(clientCanceledOrder);

            CancellationReason clientRefusedMaster = new CancellationReason();
            clientRefusedMaster.Position = -1;
            clientRefusedMaster.ReasonTechnicalName = "ClientRefusedMaster";
            clientRefusedMaster.ReasonName = "Клиент отказался от мастера";
            clientRefusedMaster.IsEditAllowed = false;
            SaveOrUpdate(clientRefusedMaster);

            CancellationReason clientAsksHandsCallback = new CancellationReason();
            clientAsksHandsCallback.Position = -1;
            clientAsksHandsCallback.ReasonTechnicalName = "ClientAsksHandsCallback";
            clientAsksHandsCallback.ReasonName = "Клиент просит Руки перезвонить";
            clientAsksHandsCallback.IsEditAllowed = false;
            SaveOrUpdate(clientAsksHandsCallback);

            CancellationReason clientNotResponding = new CancellationReason();
            clientNotResponding.Position = -1;
            clientNotResponding.ReasonTechnicalName = "ClientNotResponding";
            clientNotResponding.ReasonName = "Клиент не отвечает";
            clientNotResponding.IsEditAllowed = false;
            SaveOrUpdate(clientNotResponding);
        }


        public List<BlazorApp1.Models.CancellationReason> GetAllWithoutDefault()
        {
            return _table.Find(x => x.IsEditAllowed == true).ToList();
        }
    }

}
