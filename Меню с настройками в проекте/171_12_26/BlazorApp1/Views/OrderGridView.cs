using BlazorApp1.Models;
using BlazorApp1.Utils;

namespace BlazorApp1.Views
{
    public class OrderGridView
    {
        //для TemplateFieldGridView из которого можно взять значение и как строку и как int

        public Order Order { get; set; }

        ////03.04.23 / 05.04.23 для сохранения форматов
        //public List<TemplateFieldGridView> MainFields { get; set; }
        //public List<TemplateFieldGridView> Fields { get; set; }
        //public List<TemplateFieldGridView> ExecutorFields { get; set; }
        ////---------------------------------------------------------------------

        public List<TemplateFieldGridView> AllFields { get; set; }

        public OrderGridView(Order order)
        {
            this.Order = order;

            if (order.Template != null)
            {
                AllFields = new();
                var mainFields = new Views.MainFields(order).Fields;
                foreach (var field in mainFields)
                {
                    AllFields.Add(field);
                }

                if (order.Template.Fields != null && order.Template.Fields.Count > 0)
                {
                    //Fields = new();
                    foreach (var field in order.Template.Fields)
                    {
                        var fieldView = new TemplateFieldGridView(field);
                        //Fields.Add(fieldView);
                        AllFields.Add(fieldView);
                    }
                }

                if (order.Template.ExecutorFields != null && order.Template.ExecutorFields.Count > 0)
                {
                    //ExecutorFields = new();
                    foreach (var field in order.Template.ExecutorFields)
                    {
                        var fieldView = new TemplateFieldGridView(field);
                        //ExecutorFields.Add(fieldView);
                        AllFields.Add(fieldView);
                    }
                }

            }
        }
    }
}
