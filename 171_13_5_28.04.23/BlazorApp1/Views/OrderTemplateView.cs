using BlazorApp1.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Views
{
    //укороченный OrderTemplate где вместо полей
    //public List<TemplateField>? Fields { get; set; } = new();
    //public List<TemplateField>? ExecutorFields { get; set; } = new();
    //такие же только типа TemplateFieldGridView из которого можно взять значение и как строку и как int

    public class OrderTemplateView
    {
        public string Id { get; private set; }

        public string? OrderTemplateName { get; private set; }
        public string? OrderType { get; private set; } //Значение(не id)

        public TimeSpan? Duration
        {
            get
            {
                if (DurationHour == null && DurationMinute == null)
                    return new TimeSpan();

                return new TimeSpan((int)DurationHour, (int)DurationMinute, 0);
            }
            set
            {
                if (value == null)
                {
                    this.DurationHour = null;
                    this.DurationMinute = null;
                }
                else
                {
                    var minutes = value.Value.TotalMinutes;
                    double m = minutes % 60;
                    int h = (int)minutes / 60;

                    this.DurationHour = h;
                    this.DurationMinute = (int)m;
                }
            }
        }
        public int? DurationHour { get; private set; }
        public int? DurationMinute { get; private set; }
        public string? OrderDescription { get; private set; }

        public List<Skill>? Skills { get; private set; } //Сами Навыки

        public List<TemplateFieldGridView>? Fields { get; private set; } = new();
        public List<TemplateFieldGridView>? ExecutorFields { get; private set; } = new();

        public string Tag { get; private set; }
        public bool IsOnlyAdminEditable { get; private set; } = false;

        public OrderTemplateView(OrderTemplate template)
        {
            this.Id = template.Id;
            this.OrderTemplateName = template.OrderTemplateName;
            this.OrderType = template.OrderType;
            this.Duration = template.Duration;
            this.DurationHour = template.DurationHour;
            this.DurationMinute = template.DurationMinute;
            this.OrderDescription = template.OrderDescription;
            this.Skills = template.Skills;

            if(template.Fields != null && template.Fields.Count > 0)
            {
                foreach (var field in template.Fields)
                {
                    this.Fields.Add(new TemplateFieldGridView(field));
                }
            }

            if (template.ExecutorFields != null && template.ExecutorFields.Count > 0) 
            {
                foreach (var field in template.ExecutorFields)
                {
                    this.ExecutorFields.Add(new TemplateFieldGridView(field));
                }
            }
            
            this.Tag = template.Tag;
            this.IsOnlyAdminEditable = template.IsOnlyAdminEditable;
        }

        public override string ToString()
        {
            return OrderTemplateName?.ToString();
        }

    }
}
