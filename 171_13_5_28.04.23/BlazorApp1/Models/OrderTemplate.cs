using BlazorApp1.Models.Validation;
using BlazorApp1.Services;
using FluentValidation;
using MessagePack.Resolvers;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NodaTime.Extensions;
using System;
using System.Security.Policy;
using System.Xml.Linq;
using static MudBlazor.CategoryTypes;

namespace BlazorApp1.Models
{
    public static class FieldType
    {
        public const string FTText = "FTText";
        public const string FTFile = "FTFile";
        public const string FTLink = "FTLink";
        public const string FTList = "FTList";
        public const string FTRuble = "FTRuble";
        public const string FTDouble = "FTDouble";
        public const string FTLong = "FTLong";
        public const string FTDate = "FTDate";
        public const string FTDateTime = "FTDateTime";
        public const string FTPhoto = "FTPhoto";
        public const string FTYesNo = "FTYesNo";
    }

    public static class FieldOwner
    {
        public const string Creator = "Creator";
        public const string Executor = "Executor";
    }

    public interface IField
    {
        public string Type { get; set; }
        public string Owner { get; set; }
        public string FieldTechnicalName { get; set; }
        public string FieldName { get; set; }
    }

    public class TemplateField : IField
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string? FieldTechnicalName { get; set; } = null;
        public string FieldName { get; set; } = string.Empty;

        public string Type { get; set; }
        public string Owner { get; set; }

        public object? _value { get; set; }

        public bool IsVisibleToExecutor { get; set; } = true;
        public bool Required { get; set; } = false;

        //длядропконтейнера и сортировки
        public int Position { get; set; }
        public string Selector { get; set; } = "myselector";

        public TemplateField()
        {
        }
        public TemplateField(string name, object value, string owner)
        {
            this.FieldName = name;
            this._value = value;
            this.Owner = owner;
        }

        public override string ToString()
        {
            return FieldName.ToString();
        }
    }
    //Текст
    public class TextField : TemplateField
    {
        public TextField(string name, string value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTText;
        }
    }
    //Файл(fullPath)
    public class FileField : TemplateField
    {
        public FileField(string name, string value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTFile;
        }
    }
    //Ссылка
    public class LinkField : TemplateField
    {
        public LinkField(string name, string value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTLink;
        }
    }
    //Деньги
    public class RubleField : TemplateField
    {
        public RubleField(string name, double value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTRuble;
        }
    }
    //Число
    public class DoubleField : TemplateField
    {
        public DoubleField(string name, double value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTDouble;
        }
    }
    //ЦелоеЧисло(Long)
    public class LongField : TemplateField
    {
        public LongField(string name, long value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTLong;
        }
    }
    //Дата
    public class DateField : TemplateField
    {
        public DateField(string name, DateTime value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTDate;
        }
    }
    //Дата и время
    public class DateTimeField : TemplateField
    {
        public DateTimeField(string name, DateTime value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTDateTime;
        }
    }

    //СПИСОК
    public class ListField : TemplateField
    {
        public ListField(string name, Models.DataList value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTList;
        }
    }


    //Фото(path)
    public class PhotoField : TemplateField
    {
        public PhotoField(string name, string value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTPhoto;
        }
    }
    //ДаНет | Только у исплолнителя
    public class YesNoField : TemplateField
    {
        public YesNoField(string name, bool value, string owner) : base(name, value, owner)
        {
            Type = FieldType.FTYesNo;
        }
    }

    public class OrderTemplate : AbstractHistory, IHistoryModel<OrderTemplate>
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        public string? OrderTemplateName { get; set; }
        public string? OrderType { get; set; } //Значение(не id)

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
        [BsonIgnore]
        public int? DurationHour { get; set; }
        [BsonIgnore]
        public int? DurationMinute { get; set; }

        public string? OrderDescription { get; set; }

        public List<Skill>? Skills { get; set; } //Сами Навыки

        public List<TemplateField>? Fields { get; set; } = new();
        public List<TemplateField>? ExecutorFields { get; set; } = new();

        public string Tag { get; set; }
        public bool IsOnlyAdminEditable { get; set; } = false;

        public List<string> FindChanges(OrderTemplate newObject)
        {
            var result = new List<string>();

            if (!JsonConvert.SerializeObject(this.Id).Equals(JsonConvert.SerializeObject(newObject.Id)))
            {
                result.Add("Id");
            }
            if (!JsonConvert.SerializeObject(this.OrderTemplateName).Equals(JsonConvert.SerializeObject(newObject.OrderTemplateName)))
            {
                result.Add("Название");
            }
            if (!JsonConvert.SerializeObject(this.OrderType).Equals(JsonConvert.SerializeObject(newObject.OrderType)))
            {
                result.Add("Тип");
            }
            if (!JsonConvert.SerializeObject(this.DurationHour).Equals(JsonConvert.SerializeObject(newObject.DurationHour)))
            {
                result.Add("Продолжительность часов");
            }
            if (!JsonConvert.SerializeObject(this.DurationMinute).Equals(JsonConvert.SerializeObject(newObject.DurationMinute)))
            {
                result.Add("Продолжительность минут");
            }

            if (!JsonConvert.SerializeObject(this.Skills).Equals(newObject.Skills))
            {
                result.Add("Необходимые навыки");
            }

            if (!JsonConvert.SerializeObject(this.OrderDescription).Equals(JsonConvert.SerializeObject(newObject.OrderDescription)))
            {
                result.Add("Описание");
            }
            if (!JsonConvert.SerializeObject(this.Tag).Equals(JsonConvert.SerializeObject(newObject.Tag)))
            {
                result.Add("Tag");
            }
            if (!JsonConvert.SerializeObject(this.IsOnlyAdminEditable).Equals(JsonConvert.SerializeObject(newObject.IsOnlyAdminEditable)))
            {
                result.Add("Редактируется только администратором");
            }

            if (!JsonConvert.SerializeObject(this.Fields).Equals(JsonConvert.SerializeObject(newObject.Fields)))
            {
                result.Add("Поле(я) диспетчера");
            }

            if (!JsonConvert.SerializeObject(this.ExecutorFields).Equals(newObject.ExecutorFields))
            {
                result.Add("Поле(я) исполнителя");
            }

            return result;
        }

        public void Trim()
        {
            if (!string.IsNullOrEmpty(this.OrderTemplateName))
            {
                this.OrderTemplateName = this.OrderTemplateName.Trim();
            }

            //Названия полей Диспетчера
            if (this.Fields != null && this.Fields.Count > 0) 
            {
                foreach (var field in this.Fields)
                {
                    field.FieldName = field.FieldName.Trim();
                    if(!string.IsNullOrEmpty(field.FieldTechnicalName))
                        field.FieldTechnicalName = field.FieldTechnicalName.Trim();
                }
            }
            //Названия полей Исполнителя
            if (this.ExecutorFields != null && this.ExecutorFields.Count > 0)
            {
                foreach (var field in this.ExecutorFields)
                {
                    field.FieldName = field.FieldName.Trim();
                    if (!string.IsNullOrEmpty(field.FieldTechnicalName))
                        field.FieldTechnicalName = field.FieldTechnicalName.Trim();
                }
            }
        }

        public override string ToString()
        {
            if(this.OrderTemplateName != null)
                return this.OrderTemplateName;
            else
                return string.Empty;
        }
    }

    public class TemplateValidator : AbstractValidator<OrderTemplate>
    {
        public TemplateValidator()
        {
            //Название Шаблона
            RuleFor(x => x.OrderTemplateName).NotEmpty().WithMessage("Заполните название шаблона");
            RuleFor(x => x.OrderTemplateName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //Описание
            RuleFor(x => x.OrderDescription).MaximumLength(1000).WithMessage(ValidationMessages.TooLong);

            //На все поля диспетчера навесить свою валидацию
            RuleForEach(x => x.Fields).SetValidator(new FieldValidator());

            //На все поля исполнителя навесить свою валидацию
            RuleForEach(x => x.ExecutorFields).SetValidator(new FieldValidator());
        }
    }
    public class FieldValidator : AbstractValidator<TemplateField>
    {
        public FieldValidator() 
        {
            //Имя Поля
            RuleFor(x => x.FieldName).NotEmpty().WithMessage("Заполните название поля");
            RuleFor(x => x.FieldName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //Техническое Имя Поля
            RuleFor(x => x.FieldTechnicalName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);
        }
    }

}
