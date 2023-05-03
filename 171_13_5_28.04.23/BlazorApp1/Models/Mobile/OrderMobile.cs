using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Linq;

namespace BlazorApp1.Models.Mobile
{
    public class OrderStatusMobile //В Планадо это резолюции. Их можно настраивать
    {
        public string? StatusCode { get; set; }
        public string? StatusName { get; set; }
        public OrderStatusMobile(OrderStatus originalStatus)
        {
            this.StatusCode = originalStatus.StatusCode;
            this.StatusName = originalStatus.StatusName;
        }
    }

    public class OrderMobile
    {
        public string? Id { get; set; }
        public string? OrderNumPrefix { get; set; }
        public int? OrderNum { get; set; } = null;
        public string OrderName { get; set; }
        public ClientMobile? Client { get; set; } = null; //внутри есть адрес и контакты, но в заказе мб другие
        public List<ClientContactMobile>? Contacts { get; set; } = null;
        public AddressMobile? Address { get; set; }

        public DateTime? PlannedDateTime { get; set; } = null; //Дата и время в одной переменной

        //
        public DateTime? RoadTimeStart { get; set; }
        public DateTime? RoadTimeStop { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }

        //public List<RegionValueMobile>? Regions { get; set; }
        public List<RegionMobile>? Regions { get; set; }

        public EmployeeMobile? OrderEmployeeExecutor { get; set; } = null; //(он же ответственный)
        public OrderTemplateMobile? Template { get; set; }
        public OrderStatusMobile? OrderStatus { get; set; }
        public CancellationReasonMobile? CancellationReason { get; set; } = null;

        public bool? IsReadByExecutor { get; set; }

        public OrderMobile(Order originalOrder)
        {
            this.Id = originalOrder.Id;
            this.OrderNumPrefix = originalOrder.OrderNumPrefix;
            this.OrderNum = originalOrder.OrderNum;
            this.OrderName = originalOrder.OrderName;

            if (originalOrder.Client != null)
            {
                this.Client = new ClientMobile(originalOrder.Client);
            }else { this.Client = null; }
            
            if (originalOrder.Contacts != null && originalOrder.Contacts.Count > 0)
            {
                var contacts = new List<ClientContactMobile>();
                foreach (var contact in originalOrder.Contacts)
                {
                    contacts.Add(new ClientContactMobile(contact));
                }
                this.Contacts = contacts;
            }else { this.Contacts = null; }

            if (originalOrder.Address != null) 
            {
                this.Address = new AddressMobile(originalOrder.Address);
            }else { this.Address = null; }


            this.PlannedDateTime = originalOrder.PlannedDateTime;
            this.RoadTimeStart = originalOrder.RoadTimeStart;
            this.RoadTimeStop = originalOrder.RoadTimeStop;
            this.StartTime = originalOrder.StartTime;
            this.StopTime = originalOrder.StopTime;

            //Регионы
            if (originalOrder.Regions != null && originalOrder.Regions.Count > 0)
            {
                var regions = new List<RegionMobile>();

                var regionCount = new HashSet<string>();
                foreach (var regionValue in originalOrder.Regions)
                {
                    regionCount.Add(regionValue.RegionName);
                }

                foreach (var regionName in regionCount)
                {
                    var newRegion = new RegionMobile();
                    newRegion.RegionName = regionName;
                    List<string> selectedValues = new();
                    foreach (var regionValue in originalOrder.Regions.FindAll(x=>x.RegionName == regionName))
                    {
                        selectedValues.Add(regionValue.Value);
                        //newRegion.SelectedValues.Add(regionValue.Value);
                    }
                    newRegion.SelectedValues = selectedValues;
                    regions.Add(newRegion);
                }
                this.Regions = regions;
            }
            else { this.Regions = null; }
            //

            if (originalOrder.OrderEmployeeExecutor != null) 
            {
                this.OrderEmployeeExecutor = new EmployeeMobile(originalOrder.OrderEmployeeExecutor);
            }
            else{this.OrderEmployeeExecutor = null;}

            this.Template = new OrderTemplateMobile(originalOrder.Template);
            this.OrderStatus = new OrderStatusMobile(originalOrder.OrderStatus);

            if (originalOrder.CancellationReason != null)
            {
                this.CancellationReason = new CancellationReasonMobile(originalOrder.CancellationReason);
            }else { this.CancellationReason = null; }

            this.IsReadByExecutor = originalOrder.IsReadByExecutor;
        }
    }
}
