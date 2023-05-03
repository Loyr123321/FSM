using BlazorApp1.Models;

namespace BlazorApp1.Views
{
    public class MainFields
    {
        public List<TemplateFieldGridView> Fields = new();
        public MainFields(Order order)
        {
            //№ Заказа
            long num = -1;
            if (order.OrderNum != null)
            {
                num = (long)order.OrderNum;
            }
            var orderNum = new LongField("№ Заявки", num, "main");
            orderNum.Id = "ORDERNUM";
            Fields.Add(new TemplateFieldGridView(orderNum));

            //Имя заказа
            //if()
            //var orderName = new TextField("Имя заказа", order.OrderName, "main");
            //Fields.Add(orderName);

            //Статус
            string statusName = string.Empty;
            if (!string.IsNullOrEmpty(order.OrderStatus.StatusName))
            {
                statusName = order.OrderStatus.StatusName;
            }
            var orderStatus = new TextField("Статус", statusName, "main");
            orderStatus.Id = "ORDERSTATUS";
            Fields.Add(new TemplateFieldGridView(orderStatus));

            //Шаблон
            //var orderTemplate = new TextField("Шаблон", order.Template.OrderTemplateName, "main");
            //Fields.Add(orderTemplate);

            //Тип заказа
            string typeName = string.Empty;
            if (!string.IsNullOrEmpty(order.Template.OrderType))
            {
                typeName = order.Template.OrderType;
            }
            var orderShortType = new TextField("Тип заказа", typeName, "main"); //!! Посмотреть где записывается тип и записывать короткий
            orderShortType.Id = "ORDERSHORTTYPE";
            Fields.Add(new TemplateFieldGridView(orderShortType));

            //Описание
            string description = string.Empty;
            if (!string.IsNullOrEmpty(order.Template.OrderDescription))
            {
                description = order.Template.OrderDescription;
            }
            var orderDescription = new TextField("Описание", description, "main");
            orderDescription.Id = "ORDERDESCRIPTION";
            Fields.Add(new TemplateFieldGridView(orderDescription));

            //Адресс
            string address = string.Empty;
            if (order.Address != null)
            {
                address = order.Address.GetAddress();
            }
            var orderAddress = new TextField("Адрес", address, "main");
            orderAddress.Id = "ORDERADDRESS";
            Fields.Add(new TemplateFieldGridView(orderAddress));

            //КонтактыМастера
            string executorContact = string.Empty;
            if (order.OrderEmployeeExecutor != null && order.OrderEmployeeExecutor.Phone != null)
            {
                executorContact = order.OrderEmployeeExecutor.GetFullName() + " " + order.OrderEmployeeExecutor.Phone;
            }
            var orderExecutorContacts = new TextField("Контакты мастера", executorContact, "main");//Контакты мастера
            orderExecutorContacts.Id = "ORDEREXECUTORCONTACTS";
            Fields.Add(new TemplateFieldGridView(orderExecutorContacts));

            //Контакты заказа
            string contactString = string.Empty;
            if (order.Contacts != null && order.Contacts.Count() > 0)
            {
                var contact = order.Contacts.FirstOrDefault(x => x.IsMain);
                contactString = contact.ClientContactName + " " + contact.Phone;
            }
            var orderContacts = new TextField("Контакты заказа", contactString, "main");
            orderContacts.Id = "ORDERCONTACTS";
            Fields.Add(new TemplateFieldGridView(orderContacts));

            //Планируемая дата
            var planDateTime = new DateTime(1970, 1, 1);
            if (order.PlannedDateTime != null)
            {
                planDateTime = (DateTime)order.PlannedDateTime;
            }
            var orderPlanDateTime = new DateTimeField("Планируемая дата", planDateTime, "main");
            orderPlanDateTime.Id = "ORDERPLANDATETIME";
            Fields.Add(new TemplateFieldGridView(orderPlanDateTime));

            //Дата Закрытия
            var stopDateTime = new DateTime(1970, 1, 1);
            if (order.StopTime != null)
            {
                stopDateTime = (DateTime)order.StopTime;
            }
            var orderStopDateTime = new DateTimeField("Дата закрытия", stopDateTime, "main");
            orderStopDateTime.Id = "ORDERSTOPDATETIME";
            Fields.Add(new TemplateFieldGridView(orderStopDateTime));

            //Создал
            string creator = string.Empty;
            if (order.Creator != null)
            {
                creator = order.Creator;
            }
            var orderCreator = new TextField("Создал", creator, "main");
            orderCreator.Id = "ORDERCREATOR";
            Fields.Add(new TemplateFieldGridView(orderCreator));

            //Дата Создания
            var createDateTime = new DateTime(1970, 1, 1);
            if (order.CreateTime != null)
            {
                createDateTime = (DateTime)order.CreateTime;
            }
            var orderCreateDateTime = new DateTimeField("Дата создания", createDateTime, "main");
            orderCreateDateTime.Id = "ORDERCREATEDATETIME";
            Fields.Add(new TemplateFieldGridView(orderCreateDateTime));
        }
    }
}
