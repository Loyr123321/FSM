using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models.Mobile
{
    public class TemplateFieldMobile : IField
    {
        public string Id { get; set; }
        public string FieldName { get; set; }
        public string FieldTechnicalName { get; set; }

        public string Type { get; set; }
        public string Owner { get; set; }

        public object? _value { get; set; }

        public bool IsVisibleToExecutor { get; set; }
        public bool Required { get; set; }

        public TemplateFieldMobile(TemplateField originalField)
        {
            this.Id = originalField.Id;
            this.FieldTechnicalName = originalField.FieldTechnicalName;
            this.FieldName = originalField.FieldName;
            this.Type = originalField.Type;
            this.Owner = originalField.Owner;
            if (originalField.Type == "FTList")
            {
                this._value = new DataListMobile((DataList)originalField._value);
            }
            else
            {
                this._value = originalField._value;
            }
            this.IsVisibleToExecutor = originalField.IsVisibleToExecutor;
            this.Required = originalField.Required;
        }
    }

    public class OrderTemplateMobile
    {
        public string? Id { get; set; } = null;
        public string? OrderTemplateName { get; set; } = null;
        public string? OrderType { get; set; } = null;

        //28.02.23 TimeSpan to DateTime
        public DateTime? Duration { get; set; } = null;
        public string? OrderDescription { get; set; } = null;
        public List<string>? Skills { get; set; } = null;

        public List<TemplateFieldMobile>? fields { get; set; } = null;
        public List<TemplateFieldMobile>? executorFields { get; set; } = null;

        public OrderTemplateMobile(OrderTemplate originalOrderTemplate)
        {
            this.Id = originalOrderTemplate.Id;
            this.OrderTemplateName = originalOrderTemplate.OrderTemplateName;
            this.OrderType = originalOrderTemplate.OrderType;
            //this.Duration = originalOrderTemplate.Duration;
            this.Duration = new DateTime(year:1970, month:1, day:1 + originalOrderTemplate.Duration.Value.Days, hour: originalOrderTemplate.Duration.Value.Hours, minute: originalOrderTemplate.Duration.Value.Minutes, second: originalOrderTemplate.Duration.Value.Seconds, millisecond:0, DateTimeKind.Utc);
            this.OrderDescription = originalOrderTemplate.OrderDescription;

            if (originalOrderTemplate.Fields != null && originalOrderTemplate.Fields.Count > 0)
            {
                var fields = new List<TemplateFieldMobile>();
                foreach (var f in originalOrderTemplate.Fields)
                {
                    fields.Add(new TemplateFieldMobile(f));
                }
                this.fields = fields;
            }

            //Навыки
            if (originalOrderTemplate.Skills != null && originalOrderTemplate.Skills.Count > 0)
            {
                List<string> skills = new();
                foreach (var skill in originalOrderTemplate.Skills.OrderBy(x => x.SkillName).ToList())
                {
                    skills.Add(skill.SkillName);
                }
                this.Skills = skills;
            }

            if (originalOrderTemplate.ExecutorFields != null && originalOrderTemplate.ExecutorFields.Count > 0)
            {
                var executorFields = new List<TemplateFieldMobile>();
                foreach (var f in originalOrderTemplate.ExecutorFields)
                {
                    executorFields.Add(new TemplateFieldMobile(f));
                }
                this.executorFields = executorFields;
            }

        }
    }
}
