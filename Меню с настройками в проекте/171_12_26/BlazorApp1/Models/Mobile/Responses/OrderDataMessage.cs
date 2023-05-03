namespace BlazorApp1.Models.Mobile.Responses
{
    [Serializable]
    public class OrderDataMessage
    {
        public string OrderId { get; set; }
        public string OrderName { get; set; }
        public string StatusName { get; set; }
        public string Event { get; set; }
        public string Title { get; set; }

        public OrderDataMessage(string orderId, string orderName, string statusName, string eventName, string title)
        {
            OrderId = orderId;
            OrderName = orderName;
            StatusName = statusName;
            Event = eventName;
            Title = title;
        }
    }
}
