using BlazorApp1.Models.Validation;
using BlazorApp1.Services;
using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using static MudBlazor.FilterOperator;

namespace BlazorApp1.Models
{
    public enum EmployeeHowToNotify
    {
        Phone,
        Mail
    }

    public enum EmployeeWorkMode
    {
        ForCash,
        ByContract,
        CompanyMember
    }

    //Выездные Мастера
    public class Employee : BaseUser, IHistoryModel<Employee>
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? HowToNotify { get; set; } = EmployeeHowToNotify.Phone.ToString(); //инструкцию для активации смс или почта


        //Работает за Наличные По контракту или Сотрудник компании
        public string? WorkMode { get; set; } = EmployeeWorkMode.ForCash.ToString();

        public string? Phone { get; set; } = string.Empty;
        public string? Mail { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; } = true; //Активен или Заблокирован
        public List<OrderType>? OrderTypes { get; set; }
        public List<Skill>? Skills { get; set; } //Сами Навыки
        public Address? Address { get; set; }
        public List<RegionValue>? Regions { get; set; } //ТерриторияОбслуживания (Район/Метро)
        public string? Comment { get; set; }

        //Счетчик назначенных заявок
        public int AssignedCounter { get; set; }
        //Счетчик завершенных заявок
        public int RejectedCounter { get; set; }
        public double GetRating()
        {
            try
            {
                if (this.AssignedCounter == 0)
                {
                    return 0;
                }

                return Math.Round((1 - ((double)this.RejectedCounter / (double)this.AssignedCounter)) * 10, 3, MidpointRounding.AwayFromZero);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Employee_GetRating_Exception: " + ex.Message);
                return -1;
            }
        }

        public TFile? Photo { get; set; }

        public Contract? Contract { get; set; } = null;
        public string? MyCompany { get; set; }

        public string GenerateLogin()
        {
            Random random = new Random();

            string login0 = string.Empty;
            if (!string.IsNullOrWhiteSpace(this.LastName))
            {
                login0 = Utils.Utils.Translit(LastName).ToLower();
            }

            string login1 = string.Empty;
            if (!string.IsNullOrWhiteSpace(this.FirstName))
            {
                login1 = Utils.Utils.Translit(FirstName.Substring(0, 1).ToLower());
            }

            string login2 = string.Empty;
            if (!string.IsNullOrWhiteSpace(this.MiddleName))
            {
                login2 = Utils.Utils.Translit(MiddleName.Substring(0, 1).ToLower());
            }

            string login3 = random.Next(999).ToString();

            return login0 + "." + login1 + login2 + login3;
        }

        public string GetFullName()
        {
            if (string.IsNullOrWhiteSpace(this.LastName) && string.IsNullOrWhiteSpace(this.FirstName) && string.IsNullOrWhiteSpace(this.MiddleName))
            {
                return "NULL_NAME";
            }
            return (this.LastName + " " + this.FirstName + " " + this.MiddleName).Trim();
        }

        public bool GetCurrentStatus()
        {
            //Получить статус из планировщика на текущий момент. чтобы пон раб или нет
            return true;
        }

        public string CalculateSkillMatching(List<Skill>? employeeSkills, List<Skill>? inputSkills)
        {
            if (inputSkills == null || inputSkills.Count() == 0)
            {
                return string.Empty;
            }

            if (employeeSkills == null || employeeSkills.Count() == 0)
            {
                return "  (Навыков 0" + " из " + inputSkills.Count() + ")";
            }
            else
            {
                IEnumerable<Skill> matchingSkills = employeeSkills.Intersect(inputSkills);
                return "  (Навыков " + matchingSkills.Count() + " из " + inputSkills.Count() + ")";
            }

        }

        public string CalculateRegionMatching(List<RegionValue>? employeeRegions, List<RegionValue>? inputRegions)
        {
            if (inputRegions == null || inputRegions.Count() == 0)
            {
                return string.Empty;
            }

            if (employeeRegions == null || employeeRegions.Count() == 0)
            {
                return "  (Регионов 0" + " из " + inputRegions.Count() + ")";
            }
            else
            {
                IEnumerable<RegionValue> matchingRegions = employeeRegions.Intersect(inputRegions);
                return "  (Регионов " + matchingRegions.Count() + " из " + inputRegions.Count() + ")";
            }

        }

        public static Employee CloneEmployeeWithoutPass(Employee inputEmployee)
        {
            Employee newEmployee = new Employee();
            newEmployee.Id = inputEmployee.Id;
            newEmployee.FirstName = inputEmployee.FirstName;
            newEmployee.LastName = inputEmployee.LastName;
            newEmployee.MiddleName = inputEmployee.MiddleName;
            newEmployee.Mail = inputEmployee.Mail;
            newEmployee.Login = inputEmployee.Login;
            newEmployee.IsActive = inputEmployee.IsActive;

            return newEmployee;
        }

        public List<string> FindChanges(Employee newObject)
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
            if (!JsonConvert.SerializeObject(this.HowToNotify).Equals(JsonConvert.SerializeObject(newObject.HowToNotify)))
            {
                result.Add("Куда отправлять инструкцию");
            }
            if (!JsonConvert.SerializeObject(this.Phone).Equals(JsonConvert.SerializeObject(newObject.Phone)))
            {
                result.Add("Телефон");
            }
            if (!JsonConvert.SerializeObject(this.Mail).Equals(JsonConvert.SerializeObject(newObject.Mail)))
            {
                result.Add("Почта");
            }
            if (!JsonConvert.SerializeObject(this.Login).Equals(JsonConvert.SerializeObject(newObject.Login)))
            {
                result.Add("Логин");
            }
            if (!JsonConvert.SerializeObject(this.Password).Equals(JsonConvert.SerializeObject(newObject.Password)))
            {
                result.Add("Пароль");
            }
            if (!JsonConvert.SerializeObject(this.IsActive).Equals(JsonConvert.SerializeObject(newObject.IsActive)))
            {
                result.Add("Может выполнять заказы");
            }

            if (!JsonConvert.SerializeObject(this.Skills).Equals(JsonConvert.SerializeObject(newObject.Skills)))
            {
                result.Add("Навыки");
            }

            if (!JsonConvert.SerializeObject(this.Contract).Equals(JsonConvert.SerializeObject(newObject.Contract)))
            {
                result.Add("Договор");
            }


            if (!JsonConvert.SerializeObject(this.Address).Equals(JsonConvert.SerializeObject(newObject.Address)))
            {
                result.Add("Адрес");
            }

            if (!JsonConvert.SerializeObject(this.Regions).Equals(JsonConvert.SerializeObject(newObject.Regions)))
            {
                result.Add("Регионы");
            }

            if (!JsonConvert.SerializeObject(this.Comment).Equals(JsonConvert.SerializeObject(newObject.Comment)))
            {
                result.Add("Комментарий");
            }

            if (!JsonConvert.SerializeObject(this.Photo).Equals(JsonConvert.SerializeObject(newObject.Photo)))
            {
                result.Add("Фото");
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

            //Логин
            if (!string.IsNullOrEmpty(this.Login))
            {
                this.Login = new string(this.Login.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                this.Login = this.Login.ToLower();
            }

            //Phone ограничен маской

            //Удалить пробелы из Почты
            if (!string.IsNullOrEmpty(this.Mail))
            {
                this.Mail = this.Mail.ToLower().Trim();
            }

            //Удалить пробелы из Comment
            if (!string.IsNullOrEmpty(this.Comment))
            {
                this.Comment = this.Comment.Trim();
            }

            //Удалить пробелы из Адреса
            if (this.Address != null)
            {
                this.Address.Trim();
            }
        }
    }

    //Договор
    public class Contract 
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }

        public string? OrganizationName { get; set; }

        public string? ContractType { get; set; }

        public string GetFullName()
        {
            if (this.ContractType.Equals(Models.ContractType.Physical.ToString()) || this.ContractType.Equals(Models.ContractType.IndividualEntrepreneur.ToString())) 
            {
                if (string.IsNullOrWhiteSpace(this.LastName) && string.IsNullOrWhiteSpace(this.FirstName) && string.IsNullOrWhiteSpace(this.MiddleName))
                {
                    return "NULL_NAME";
                }
                return (this.LastName + " " + this.FirstName + " " + this.MiddleName).Trim();
            }
            else 
            {
                if (string.IsNullOrWhiteSpace(this.OrganizationName))
                {
                    return "NULL_NAME";
                }
                return (this.OrganizationName).Trim();
            }
        }
    }
    public enum ContractType
    {
        Physical, //ФизЛицо
        IndividualEntrepreneur, //ИП
        LimitedLiabilityCompany, //ООО
    }

    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            //FirstName
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Заполните Имя");
            RuleFor(x => x.FirstName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //LastName
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Заполните Фамилию");
            RuleFor(x => x.LastName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //MiddleName
            //RuleFor(x => x.MiddleName).NotEmpty().WithMessage("Заполните Отчество");
            RuleFor(x => x.MiddleName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //Телефон
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Заполните Телефон").When(x => x.HowToNotify == EmployeeHowToNotify.Phone.ToString());
            RuleFor(x => x.Phone).Length(17).WithMessage("Телефон неверный").When(x => x.HowToNotify == EmployeeHowToNotify.Phone.ToString());

            //Почта
            RuleFor(x => x.Mail).NotEmpty().WithMessage("Заполните Почта").When(x => x.HowToNotify == EmployeeHowToNotify.Mail.ToString());
            RuleFor(x => x.Mail).MaximumLength(100).WithMessage(ValidationMessages.TooLong).When(x => x.HowToNotify == EmployeeHowToNotify.Mail.ToString());

            //Комментарий
            RuleFor(x => x.Comment).MaximumLength(1100).WithMessage(ValidationMessages.TooLong);
        }
    }
}
