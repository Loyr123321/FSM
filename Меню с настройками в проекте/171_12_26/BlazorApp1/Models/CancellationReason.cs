using BlazorApp1.Services;
using MongoDB.Bson.Serialization.Attributes;
using static NPOI.HSSF.Util.HSSFColor;

namespace BlazorApp1.Models
{
    //Причина Отклонения(отмены заказа)
    public class CancellationReason : IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]

        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        //длядропконтейнера

        public int Position { get; set; }

        public string Selector { get; set; } = "myselector";

        public string ReasonName { get; set; } = string.Empty;

        //Пояснения к таблице отклонений
        public string ReasonNote { get; set; } = string.Empty;

        public string ReasonTechnicalName { get; set; } //Технические имена
        public bool IsEditAllowed { get; set; } = true; //Для запрета удаления

        public override string ToString()
        {
            if (this.ReasonName == null)
            {
                return string.Empty;
            }
            return ReasonName.ToString();
        }
        public override bool Equals(object? obj)
        {
            var item = obj as CancellationReason;
            if (item == null)
            {
                return false;
            }

            return this.ReasonName.Equals(item.ReasonName);
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

    }
}
