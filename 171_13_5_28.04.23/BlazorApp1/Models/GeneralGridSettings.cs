using BlazorApp1.Services;
using BlazorApp1.Views;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorApp1.Models
{
    public class GeneralGridSettings : IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string TemplateId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; } //Дефолтный. Только 1. Который будет загрузаться когда у человека нет его личного основного шаблона
        
        //Для уникальности
        public string DefaultUniqueness
        {
            get
            {
                if (IsDefault==true)
                {
                    return "Default";
                }
                else
                {
                    return this.Id;
                }
            }
            private set{}
        }

        public List<TemplateFieldGridView> MainFields { get; set; }
        public List<TemplateFieldGridView> Fields { get; set; }
        public List<TemplateFieldGridView> ExecutorFields { get; set; }

        public GeneralGridSettings(bool isDefault, string templateId, string name, List<TemplateFieldGridView> mainFields, List<TemplateFieldGridView> fields, List<TemplateFieldGridView> executorFields)
        {
            this.TemplateId = templateId;
            this.Name = name;
            this.MainFields = mainFields;
            this.Fields = fields;
            this.ExecutorFields = executorFields;

            if (isDefault == true)
            {
                this.IsDefault = true;
                DefaultUniqueness = "DEFAULT";
            }
            else
            {
                this.IsDefault = false;
                DefaultUniqueness = this.Id;
            }
        }

        public UserGridSettings ToUserGridSettings(string userId, bool isMain, bool isDefault)
        {
            return new UserGridSettings(this.Id, userId, this.TemplateId, this.Name, this.MainFields, this.Fields, this.ExecutorFields, isMain, isGeneral: true, isDefault: isDefault);
        }

        public override string ToString()
        {
            return this.Name.ToString();
        }
    }
}
