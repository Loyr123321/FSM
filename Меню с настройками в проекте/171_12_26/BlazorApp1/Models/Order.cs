using BlazorApp1.Services;
using BlazorApp1.Utils;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace BlazorApp1.Models
{
    public class OrderStatus //В Планадо это резолюции. Их можно настраивать
    {
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public int Position { get; set; }

        public static List<OrderStatus> statuses= new List<OrderStatus>();
        public OrderStatus(string statusCode, string statusName, int position)
        {
            this.StatusCode = statusCode;
            this.StatusName = statusName;
            this.Position = position;

            statuses.Add(this);
        }

        public static readonly OrderStatus NEW = new OrderStatus("NEW", "Новая заявка", 0);
        public static readonly OrderStatus ACCEPTED = new OrderStatus("ACCEPTED", "Принято", 1);
        public static readonly OrderStatus INROAD = new OrderStatus("INROAD", "В пути", 2);
        public static readonly OrderStatus ACTIVE = new OrderStatus("ACTIVE", "В работе", 3);

        public static readonly OrderStatus REJECTED = new OrderStatus("REJECTED", "Отклонено", 4);
        public static readonly OrderStatus CANCELED = new OrderStatus("CANCELED", "Отменено", 5);

        public static readonly OrderStatus SUSPENDED = new OrderStatus("SUSPENDED", "Приостановлено", 6);
        public static readonly OrderStatus CHANGED = new OrderStatus("CHANGED", "Изменено", 7);
        public static readonly OrderStatus COMPLETED = new OrderStatus("COMPLETED", "Завершено", 8);
        public static readonly OrderStatus DELETED = new OrderStatus("DELETED", "Удалено", 9);

        public override bool Equals(object? obj)
        {
            var item = obj as OrderStatus;
            if (item == null)
            {
                return false;
            }

            return this.StatusCode.Equals(item.StatusCode);
        }
        //public override int GetHashCode()
        //{
        //    return this.StatusCode.GetHashCode();
        //}
        public override string ToString()
        {
            return this.StatusName;
        }
    }

    public class Order : AbstractHistory, IHistoryModel<Order>
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string? OrderNumPrefix { get; set; } = null;
        public int? OrderNum { get; set; } = null;
        public string OrderName { get; set; }

        public Client? Client { get; set; } = null; //внутри есть адрес и контакты, но в заказе мб другие
        public List<ClientContact>? Contacts { get; set; } = null;
        public Address? Address { get; set; }

        //Дата создания (поступления) заявки 09.03.23
        public DateTime? CreateDateTime { get; set; } = DateTime.Now;

        public DateTime? PlannedDateTime
        {
            get
            {
                return PlannedDate + PlannedTime;
            }
            set
            {
                if (value.HasValue)
                {
                    PlannedDate = value.Value.Date;
                    PlannedTime = value.Value.TimeOfDay;
                }
                else
                {
                    PlannedDate = null;
                    PlannedTime = null;
                }
            }
        }
        [BsonIgnore]
        public DateTime? PlannedDate { get; set; }
        [BsonIgnore]
        public TimeSpan? PlannedTime { get; set; }

        //
        public DateTime? RoadTimeStart { get; set; } = null;
        public DateTime? RoadTimeStop { get; set; } = null;
        public DateTime? StartTime { get; set; } = null;
        public DateTime? StopTime { get; set; } = null; //Дата закрытия заявки

        public List<RegionValue>? Regions { get; set; } //ТерриторияОбслуживания (Район)(только для фильтра Сотрудников)
        public Employee? OrderEmployeeExecutor { get; set; } = null; //(он же ответственный)
        //public Brigade OrderBrigadeExecutor { get; set; }//Бригады

        public OrderTemplate? Template { get; set; }

        public OrderStatus? OrderStatus { get; set; } = OrderStatus.NEW;
        public CancellationReason? CancellationReason { get; set; } = null;
        public bool? IsReadByExecutor { get; set; } = false; //Прочитано Исполнителем 01.11.22

        public Order(OrderTemplate template)
        {
            this.Template = template;
        }

        public List<string> FindChanges(Order newObject)
        {
            var result = new List<string>();

            if (!JsonConvert.SerializeObject(this.Id).Equals(JsonConvert.SerializeObject(newObject.Id)))
            {
                result.Add("Id");
            }
            if (!JsonConvert.SerializeObject(this.OrderName).Equals(JsonConvert.SerializeObject(newObject.OrderName)))
            {
                result.Add("Имя");
            }
            if (!JsonConvert.SerializeObject(this.OrderNumPrefix).Equals(JsonConvert.SerializeObject(newObject.OrderNumPrefix)))
            {
                result.Add("NumPrefix");
            }
            if (!JsonConvert.SerializeObject(this.OrderNum).Equals(JsonConvert.SerializeObject(newObject.OrderNum)))
            {
                result.Add("№");
            }

            if (!JsonConvert.SerializeObject(this.OrderStatus).Equals(JsonConvert.SerializeObject(newObject.OrderStatus)))
            {
                result.Add("Статус");
            }

            if (!JsonConvert.SerializeObject(this.Client).Equals(JsonConvert.SerializeObject(newObject.Client)))
            {
                result.Add("Клиент");
            }

            if (!JsonConvert.SerializeObject(this.Address).Equals(JsonConvert.SerializeObject(newObject.Address)))
            {
                result.Add("Адрес");
            }

            if (!JsonConvert.SerializeObject(this.Contacts).Equals(JsonConvert.SerializeObject(newObject.Contacts)))
            {
                result.Add("Контакты");
            }

            if (!JsonConvert.SerializeObject(this.OrderEmployeeExecutor).Equals(JsonConvert.SerializeObject(newObject.OrderEmployeeExecutor)))
            {
                result.Add("Исполнитель");
            }

            if (!JsonConvert.SerializeObject(this.CreateDateTime).Equals(JsonConvert.SerializeObject(newObject.CreateDateTime)))
            {
                result.Add("Дата создания(поступления) заявки");
            }

            if (!JsonConvert.SerializeObject(this.PlannedDate).Equals(JsonConvert.SerializeObject(newObject.PlannedDate)))
            {
                result.Add("Планируемая дата");
            }

            if (!JsonConvert.SerializeObject(this.PlannedTime).Equals(JsonConvert.SerializeObject(newObject.PlannedTime)))
            {
                result.Add("Планируемое время");
            }

            if (!JsonConvert.SerializeObject(this.RoadTimeStart).Equals(JsonConvert.SerializeObject(newObject.RoadTimeStart)))
            {
                result.Add("Время на дорогу (Н)");
            }

            if (!JsonConvert.SerializeObject(this.RoadTimeStop).Equals(JsonConvert.SerializeObject(newObject.RoadTimeStop)))
            {
                result.Add("Время на дорогу (К)");
            }

            if (!JsonConvert.SerializeObject(this.StartTime).Equals(JsonConvert.SerializeObject(newObject.StartTime)))
            {
                result.Add("Время начала работы");
            }

            if (!JsonConvert.SerializeObject(this.StopTime).Equals(JsonConvert.SerializeObject(newObject.StopTime)))
            {
                result.Add("Время окончания работы");
            }

            if (!JsonConvert.SerializeObject(this.Regions).Equals(JsonConvert.SerializeObject(newObject.Regions)))
            {
                result.Add("Регионы");
            }

            if (!JsonConvert.SerializeObject(this.Template.OrderType).Equals(JsonConvert.SerializeObject(newObject.Template.OrderType))) 
            {
                result.Add("Тип заказа");
            }

            if (!JsonConvert.SerializeObject(this.Template.OrderDescription).Equals(JsonConvert.SerializeObject(newObject.Template.OrderDescription)))
            {
                result.Add("Описание");
            }

            if (!JsonConvert.SerializeObject(this.Template.Duration).Equals(JsonConvert.SerializeObject(newObject.Template.Duration)))
            {
                result.Add("Продолжительность");
            }

            if (!JsonConvert.SerializeObject(this.Template.Skills).Equals(JsonConvert.SerializeObject(newObject.Template.Skills)))
            {
                result.Add("Необходимые навыки");
            }

            //////////var tt0 = JsonConvert.SerializeObject(this.Template.Fields);
            //////////var tt1 = JsonConvert.SerializeObject(newObject.Template.Fields);
            //////////var test1 = tt0.Equals(tt1);

            //////////DateTime tt2 = (DateTime)this.Template.Fields[0]._value;
            //////////DateTime tt3 = (DateTime)newObject.Template.Fields[0]._value;
            //////////var test2 = tt2.Equals(tt3);

            //////////if (!JsonConvert.SerializeObject(this.Template.Fields).Equals(JsonConvert.SerializeObject(newObject.Template.Fields)))
            //////////{
            //////////    result.Add("Поле(я) диспетчера");
            //////////}

            if (!JsonConvert.SerializeObject(this.Template.ExecutorFields).Equals(JsonConvert.SerializeObject(newObject.Template.ExecutorFields)))
            {
                result.Add("Поле(я) исполнителя");
            }

            if (!JsonConvert.SerializeObject(this.CancellationReason).Equals(JsonConvert.SerializeObject(newObject.CancellationReason)))
            {
                result.Add("Причина отмены");
            }

            return result;
        }
    }
}
