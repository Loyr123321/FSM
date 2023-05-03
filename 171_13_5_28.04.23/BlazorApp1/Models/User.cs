using BlazorApp1.Models.Validation;
using BlazorApp1.Services;
using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Policy;

namespace BlazorApp1.Models
{
    public class User : BaseUser, IModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [BsonIgnore]
        public string ConfirmPassword { get; set; }

        public bool IsActive { get; set; } = true; //Активен или Заблокирован

        public List<string>? Roles { get; set;}

        //Id основного формата таблицы заказов
        //public string? MainUserGridSettingsId { get; set; } = null;
        public string? MyCompany { get; set; }

        public User() { }
        public User(string id, string firstName, string lastName, string middleName, string email, List<string> roles, string? mycompany)
        {
            this.Id = Id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleName = middleName;
            this.Email = email;
            this.Roles = roles;
            this.MyCompany = mycompany;
        }
        public static User CloneUserWithoutPass(User inputUser) 
        {
            User newUser = new();
            newUser.Id = inputUser.Id;
            newUser.FirstName = inputUser.FirstName;
            newUser.LastName = inputUser.LastName;
            newUser.MiddleName = inputUser.MiddleName;
            newUser.Email = inputUser.Email;
            newUser.IsActive = inputUser.IsActive;
            newUser.Roles = inputUser.Roles;
            newUser.MyCompany= inputUser.MyCompany;

            return newUser;
        }

        public void EditFromView(UserView view)
        {
            this.FirstName = view.FirstName;
            this.LastName = view.LastName;
            this.MiddleName = view.MiddleName;
            this.Email = view.Email;
            this.IsActive = view.IsActive;
            this.Roles = view.Roles;
            this.MyCompany = view.MyCompany;
        }

        public void EditFromPasswordView(UserPasswordView view)
        {
            this.Password = view.Password;
            this.ConfirmPassword = view.ConfirmPassword;
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
            if (!string.IsNullOrEmpty(this.MyCompany))
            {
                this.MyCompany = this.MyCompany.Trim();
            }

            //ЛогинПочта изменяется в SendRegisterPage(UserRegisterView)
        }

        public string GetFullName()
        {
            return this.LastName + " " + this.FirstName + " " + this.MiddleName;
        }
    }

    //Валидатор Для юзера
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            //FirstName
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Заполните Имя");
            RuleFor(x => x.FirstName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //LastName
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Заполните Фамилию");
            RuleFor(x => x.LastName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //MiddleName
            //RuleFor(x => x.MiddleName).NotEmpty().WithMessage("Заполните Отчество"); //Отчество не обязательно
            RuleFor(x => x.MiddleName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //Почта Проверка почты не нужна тк она будет проверена в классе для реги
            //RuleFor(x => x.Email).NotEmpty().WithMessage("Заполните Почту").EmailAddress().WithMessage("Введите верный Email");;
            //RuleFor(x => x.Email).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //Пароль
            RuleFor(x => x.Password).NotEmpty().WithMessage(" ");
            RuleFor(x => x.Password).MaximumLength(100).WithMessage(ValidationMessages.TooLong);
            //
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Заполните Пароль");
            RuleFor(x => x.ConfirmPassword).MaximumLength(100).WithMessage(ValidationMessages.TooLong);
            //
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Пароли не совпадают");
        }


    }




    //Для Формы регистрации через почту (Только логин и роли)
    public class UserRegisterView
    {
        public string LoginMail { get; set; }
        public IEnumerable<string> Roles = new HashSet<string>();
        public void Trim()
        {
            //Удалить пробелы из почты_логина
            if (!string.IsNullOrEmpty(this.LoginMail))
            {
                this.LoginMail = new string(this.LoginMail.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                this.LoginMail = this.LoginMail.ToLower();
            }

        }
    }
    public class UserRegisterViewValidator : AbstractValidator<UserRegisterView>
    {
        public UserRegisterViewValidator()
        {
            //Login(Email)
            RuleFor(x => x.LoginMail).NotEmpty().WithMessage("Заполните Логин(Почту)").EmailAddress().WithMessage("Введите верный Email");
            RuleFor(x => x.LoginMail).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //Roles(Роли)
            RuleFor(x => x.Roles).NotEmpty().WithMessage("Заполните Роли");
        }
    }




    //Для Формы подтверждения регистрации. (Юзер с ролями но без пароля)
    public class UserView
    {
        public string Id { get; set; } // = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true; //Активен или Заблокирован
        public List<string>? Roles { get; set; }
        public string? MyCompany { get; set; }

        public UserView()
        {
        }

        public UserView(User inputUser)
        {
            this.Id = inputUser.Id;
            this.FirstName = inputUser.FirstName;
            this.LastName = inputUser.LastName;
            this.MiddleName = inputUser.MiddleName;
            this.Email = inputUser.Email;
            this.IsActive = inputUser.IsActive;
            this.Roles = inputUser.Roles;
            this.MyCompany = inputUser.MyCompany;
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
            if (!string.IsNullOrEmpty(this.MyCompany))
            {
                this.MyCompany = this.MyCompany.Trim();
            }
            //ЛогинПочта изменяется в SendRegisterPage(UserRegisterView)
        }
    }
    public class UserViewValidator : AbstractValidator<UserView>
    {
        public UserViewValidator()
        {
            //FirstName
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Заполните Имя");
            RuleFor(x => x.FirstName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //LastName
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Заполните Фамилию");
            RuleFor(x => x.LastName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //MiddleName
            //RuleFor(x => x.MiddleName).NotEmpty().WithMessage("Заполните Отчество"); //Отчество не обязательно
            RuleFor(x => x.MiddleName).MaximumLength(100).WithMessage(ValidationMessages.TooLong);
        }
    }

    //Для смены пароля
    public class UserPasswordView
    {
        //public string Id { get; set; }
        public string OldPass { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
    public class UserPasswordViewValidator : AbstractValidator<UserPasswordView>
    {
        public UserPasswordViewValidator()
        {

            RuleFor(x => x.OldPass).NotEmpty().WithMessage("Заполните Пароль").Matches(@"\A\S+\z");
            RuleFor(x => x.OldPass).MaximumLength(100).WithMessage(ValidationMessages.TooLong);

            //Пароль
            RuleFor(x => x.Password).NotEmpty().WithMessage(" ");
            RuleFor(x => x.Password).MaximumLength(100).WithMessage(ValidationMessages.TooLong);
            //
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Заполните Новый Пароль");
            RuleFor(x => x.ConfirmPassword).MaximumLength(100).WithMessage(ValidationMessages.TooLong);
            //
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Пароли не совпадают");
        }
    }

//TheEnd
}
