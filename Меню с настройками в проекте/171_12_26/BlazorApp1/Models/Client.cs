using BlazorApp1.Models.Validation;
using BlazorApp1.Services;
using DaData.Models.Enums;
using ExpressiveAnnotations.Attributes;
using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static NodaTime.TimeZones.TzdbZone1970Location;

namespace BlazorApp1.Models
{
    public class ClientType
    {
        public string TypeId { get; set; }
        public string TypeName { get; set; }

        public static List<ClientType> clientTypes = new List<ClientType>();
        public ClientType(string typeid, string typename)
        {
            this.TypeId = typeid;
            this.TypeName = typename;

            clientTypes.Add(this);
        }

        public static readonly ClientType Physical = new ClientType("Physical", "Физлицо");
        public static readonly ClientType Legal = new ClientType("Legal", "Юрлицо");

        public override bool Equals(object? obj)
        {
            var item = obj as ClientType;
            if (item == null)
            {
                return false;
            }
            return this.TypeName.Equals(item.TypeName);
        }
    }

    public class Client : AbstractHistory, IHistoryModel<Client>
    { 
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        //PhysicalClient
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string ClientPhone { get; set; }

        public string ClientMail{ get; set; }
        //EndPhysicalClient


        //LegalClient
        public string OrganizationName { get; set; }
        //EndLegalClient

        public Address Address { get; set; }
        public List<ClientContact> Contacts { get; set; }
        public ClientType ClientType { get; set; } = ClientType.Physical;

        public string MyCompany { get; set; }
        public Client() {}

        public string GetFullName()
        {
            if (this.ClientType == null)
            {
                return "NULL_NAME";
            }

            if (this.ClientType.TypeId == "Physical")
            {
                return this.LastName + " " + this.FirstName + " " + this.MiddleName;
            }
            if (this.ClientType.TypeId == "Legal")
            {
                return this.OrganizationName;
            }
            return "NULL_NAME";
        }

        public List<string> FindChanges(Client newObject)
        {
            var result = new List<string>();

            if (!JsonConvert.SerializeObject(this.Id).Equals(JsonConvert.SerializeObject(newObject.Id)))
            {
                result.Add("Id");
            }
            if (!JsonConvert.SerializeObject(this.FirstName).Equals(JsonConvert.SerializeObject(newObject.FirstName)))
            {
                result.Add("Имя");
            }
            if (!JsonConvert.SerializeObject(this.LastName).Equals(JsonConvert.SerializeObject(newObject.LastName)))
            {
                result.Add("Фамилия");
            }
            if (!JsonConvert.SerializeObject(this.MiddleName).Equals(JsonConvert.SerializeObject(newObject.MiddleName)))
            {
                result.Add("Отчество");
            }
            if (!JsonConvert.SerializeObject(this.ClientPhone).Equals(JsonConvert.SerializeObject(newObject.ClientPhone)))
            {
                result.Add("Телефон");
            }
            if (!JsonConvert.SerializeObject(this.ClientMail).Equals(JsonConvert.SerializeObject(newObject.ClientMail)))
            {
                result.Add("Почта");
            }

            if (!JsonConvert.SerializeObject(this.OrganizationName).Equals(JsonConvert.SerializeObject(newObject.OrganizationName)))
            {
                result.Add("Наименование");
            }

            if (!JsonConvert.SerializeObject(this.Address).Equals(JsonConvert.SerializeObject(newObject.Address)))
            {
                result.Add("Адрес");
            }

            if (!JsonConvert.SerializeObject(this.Contacts).Equals(JsonConvert.SerializeObject(newObject.Contacts)))
            {
                result.Add("Контакт(ы)");
            }

            if (!JsonConvert.SerializeObject(this.ClientType).Equals(JsonConvert.SerializeObject(newObject.ClientType)))
            {
                result.Add("Тип клиента");
            }

            if (!JsonConvert.SerializeObject(this.MyCompany).Equals(JsonConvert.SerializeObject(newObject.MyCompany)))
            {
                result.Add("MyCompany");
            }

            return result;
        }

        public void Trim() 
        {
            //Удалить пробелы из ФИО
            if (!string.IsNullOrEmpty(this.FirstName))
            {
                this.FirstName = new string(this.FirstName.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
            }
            if (!string.IsNullOrEmpty(this.LastName))
            {
                this.LastName = new string(this.LastName.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
            }
            if (!string.IsNullOrEmpty(this.MiddleName))
            {
                this.MiddleName = new string(this.MiddleName.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
            }

            //Юрлицо наименование
            if (!string.IsNullOrEmpty(this.OrganizationName))
            {
                this.OrganizationName = this.OrganizationName.Trim();
            }

            //Удалить пробелы из Почты
            if (!string.IsNullOrEmpty(this.ClientMail))
            {
                this.ClientMail = this.ClientMail.ToLower().Trim();
            }

            //Удалить пробелы из Доп Контактов
            if (this.Contacts != null && this.Contacts.Count > 0)
            {
                foreach (var contact in this.Contacts)
                {
                    if (contact != null) 
                    {
                        if (!string.IsNullOrEmpty(contact.ClientContactName))
                            contact.ClientContactName = contact.ClientContactName.Trim();

                        if (!string.IsNullOrEmpty(contact.Mail))
                            contact.Mail = contact.Mail.ToLower().Trim();
                    }
                }
            }

            //Удалить пробелы из Адреса
            if (this.Address != null) 
            {
                this.Address.Trim();
            }
        }
    }

    public class ClientContact
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string ClientContactName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Mail { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public ClientContact(){}
        public ClientContact(bool ismain)
        {
            this.IsMain = ismain;
        }
        public ClientContact(string name, string phone, string mail, bool ismain)
        {
            this.ClientContactName = name;
            this.Phone = phone;
            this.Mail = mail;
            this.IsMain = ismain;
        }
    }

    public class ClientValidator : AbstractValidator<Client>
    {
        public ClientValidator() 
        {
            //FirstName
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Заполните Имя").When(x => x.ClientType == ClientType.Physical);
            RuleFor(x => x.FirstName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //LastName
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Заполните Фамилию").When(x => x.ClientType == ClientType.Physical);
            RuleFor(x => x.LastName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //MiddleName
            RuleFor(x => x.MiddleName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //ClientPhone
            RuleFor(x => x.ClientPhone).NotEmpty().WithMessage("Заполните Телефон").When(x => x.ClientType == ClientType.Physical);
            RuleFor(x => x.ClientPhone).Length(17).WithMessage("Телефон неверный");

            //ClientMail
            RuleFor(x => x.ClientMail).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //OrganizationName
            RuleFor(x => x.OrganizationName).NotEmpty().WithMessage("Заполните Наименование").When(x => x.ClientType == ClientType.Legal);
            RuleFor(x => x.OrganizationName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //MyCompany
            RuleFor(x => x.MyCompany).MaximumLength(100).WithMessage(ValidationMessages.TooLong);


            //На все контакты навесить свою валидацию
            RuleForEach(x => x.Contacts).SetValidator(new ClientContactsValidator());
        }
    }

    public class ClientContactsValidator: AbstractValidator<ClientContact>
    {
        public ClientContactsValidator() 
        {
            //ClientContactName(ФИО)
            RuleFor(x => x.ClientContactName).MaximumLength(150).WithMessage(ValidationMessages.TooLong);

            //Phone
            RuleFor(x => x.Phone).Length(17).WithMessage("Телефон доп контакта неверный");

            //Mail
            RuleFor(x => x.Mail).MaximumLength(100).WithMessage(ValidationMessages.TooLong);
        }
    }

}
