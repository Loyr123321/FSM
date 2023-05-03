using BlazorApp1.Models;
using BlazorApp1.Utils;

namespace BlazorApp1.Views
{
    public class OrderView
    {
        public Order Order { get; set; }

        //public DateTime? PlannedDate { get; set; } //во view DateTime в model int

        public OrderType? OrderType { get; set; } //Здесь OrderType в модели(в шаблоне) string

        public List<TemplateFieldView> Fields { get; set; }
        public List<TemplateFieldView> ExecutorFields { get; set; }

        public OrderView(Order order)
        {
            this.Order = order;
            if(order == null){return;}
            //if (order.PlannedDate != null)
            //{
            //    this.PlannedDate = order.PlannedDate;
            //}


            if (order.Template != null)
            {
                if (order.Template.Fields != null && order.Template.Fields.Count > 0)
                {
                    var fields = new List<TemplateFieldView>();
                    foreach (var field in order.Template.Fields)
                    {
                        var fieldView = new TemplateFieldView(field);
                        fields.Add(fieldView);
                    }
                    this.Fields = fields;
                }

                if (order.Template.ExecutorFields != null && order.Template.ExecutorFields.Count > 0)
                {
                    var executorFields = new List<TemplateFieldView>();
                    foreach (var field in order.Template.ExecutorFields)
                    {
                        var fieldView = new TemplateFieldView(field);
                        executorFields.Add(fieldView);
                    }
                    this.ExecutorFields = executorFields;
                }
            }
        }

        public void SaveViewDataToModelData()
        {
            ////if (this.PlannedDate != null)
            ////{
            ////    this.Order.PlannedDate = (DateTime)this.PlannedDate;
            ////}

            this.Order.PlannedDateTime = this.Order.PlannedDate + this.Order.PlannedTime;

            if (this.OrderType != null) 
            {
                this.Order.Template.OrderType = this.OrderType.TypeName;
            }

            if (this.Fields != null && this.Fields.Count > 0)
            {
                var fields = new List<TemplateField>();
                foreach (var field in this.Fields)
                {
                    var templateField = field.GetTemplateField();
                    fields.Add(templateField);
                }
                this.Order.Template.Fields = fields;
            }

            if (this.ExecutorFields != null && this.ExecutorFields.Count > 0)
            {
                var fields = new List<TemplateField>();
                foreach (var field in this.ExecutorFields)
                {
                    var templateField = field.GetTemplateField();
                    fields.Add(templateField);
                }
                this.Order.Template.ExecutorFields = fields;
            }
        }
    }
}
