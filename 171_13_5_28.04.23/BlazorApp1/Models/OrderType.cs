using BlazorApp1.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models
{
    public class OrderType : IModel //Типы заказов(Н-р: Первичный выезд, Монтаж входной двери ) | Опционально | просто string | Задается в Шаблонах
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public int Position { get; set; }
        public string TypeName { get; set; }
        public string ShortName { get; set; }
        public string Selector { get; set; } = "myselector";

        public OrderType(string name, string shortName)
        {
            this.TypeName = name;
            this.ShortName = shortName;
        }

        //Для отображения не загрузившейся модели (BlazorApp1.Models.OrderType)
        public override string ToString()
        {
            return this.TypeName + " ("+this.ShortName+ ")";
        }
        //public override bool Equals(object obj)
        //{
        //    if (obj == null)
        //    {
        //        return false;
        //    }
        //    if (!(obj is OrderType))
        //    {
        //        return false;
        //    }
        //    return this.TypeName == ((OrderType)obj).TypeName;
        //}
        //public override int GetHashCode()
        //{
        //    return TypeName.GetHashCode();
        //}

    }
}
