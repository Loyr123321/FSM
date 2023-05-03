using BlazorApp1.Services;
using BlazorApp1.Views;
using MongoDB.Bson.Serialization.Attributes;
using NPOI.SS.UserModel;

namespace BlazorApp1.Models
{
    //Формат (Включает шаблон и фильтры)
    public class UserGridSettings : IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string UserId { get; set; }
        public string TemplateId { get; set; }
        public string Name { get; set; }
        //public List<TemplateFieldGridView> AllFields { get; set; } //Выбранные колонки c фильтрами
        public List<TemplateFieldGridView> MainFields { get; set; }
        public List<TemplateFieldGridView> Fields { get; set; }
        public List<TemplateFieldGridView> ExecutorFields { get; set; }

        //Основной формат
        public bool IsMain { get; set; } = true;
        //Для уникальности
        public string MainUniqueness
        {
            get
            {
                if (IsMain == true)
                {
                    return "Main";
                }
                else
                {
                    return this.Id;
                }
            }
            private set { }
        }

        public bool IsGeneral { get; set; } = false; //Для определения пользовательский или Общий

        public bool IsDefault { get; set; } = false; //Для определения стандартный или нет

        //Поиск по точному совпадению или по частичному
        public bool IsExactMatch { get; set; } = false;

        public UserGridSettings() {}
        public UserGridSettings(string userId, string templateId, string name,
            List<TemplateFieldGridView> mainFields,
            List<TemplateFieldGridView> fileds,
            List<TemplateFieldGridView> executorFields,
            bool isMain,
            bool isGeneral,
            bool isDefault
        )
        {
            this.UserId = userId;
            this.TemplateId = templateId;
            this.Name = name;

            //this.AllFields = allFields;
            this.MainFields = mainFields;
            this.Fields = fileds;
            this.ExecutorFields = executorFields;
            this.IsMain = isMain;
            this.IsGeneral = isGeneral;
            this.IsDefault = isDefault;
        }

        public UserGridSettings(string id, string userId, string templateId, string name,
            List<TemplateFieldGridView> mainFields,
            List<TemplateFieldGridView> fileds,
            List<TemplateFieldGridView> executorFields,
            bool isMain,
            bool isGeneral,
            bool isDefault
        )
        {
            this.Id = id;
            this.UserId = userId;
            this.TemplateId = templateId;
            this.Name = name;

            //this.AllFields = allFields;
            this.MainFields = mainFields;
            this.Fields = fileds;
            this.ExecutorFields = executorFields;
            this.IsMain = isMain;
            this.IsGeneral = isGeneral;
            this.IsDefault = isDefault;
        }

        public override string ToString()
        {
            if (IsGeneral)
            {
                return this.Name + " (Общий)";
            }

            return this.Name;
        }

    }

    public enum GridSettingsDialogType
    {
        New,
        Save
    }
}
