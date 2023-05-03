using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using NuGet.Protocol;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorApp1.Auth
{
    public static class AuthOptions
    {
        private static IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public static readonly string ISSUER = configuration.GetValue<string>("JsonWebToken:Issuer"); // издатель токена
        public static readonly string AUDIENCE = configuration.GetValue<string>("JsonWebToken:Audience"); // потребитель токена

        private static readonly string REFRESH_KEY = configuration.GetValue<string>("JsonWebToken:RefreshKey");//refresh
        private static readonly string KEY = configuration.GetValue<string>("JsonWebToken:Key"); // ключ для шифрации токенов access
        private static readonly string REGISTER_KEY = configuration.GetValue<string>("JsonWebToken:RegisterKey"); // ключ для шифрации токенов регистрации
        private static readonly string WEB_LOGIN_KEY = configuration.GetValue<string>("JsonWebToken:WebLoginKey"); // ключ для шифрации токенов входа

        public static SymmetricSecurityKey GetSymmetricSecurityRefreshKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(REFRESH_KEY));
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        public static SymmetricSecurityKey GetSymmetricSecurityRegisterKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(REGISTER_KEY));
        public static SymmetricSecurityKey GetSymmetricSecurityWebLoginKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(WEB_LOGIN_KEY));

        public enum JWT_TYPE
        {
            Access = 0,
            Refresh = 1,
            Register = 2,
            RegisterResetPass = 22,
            WebLogin = 3
        }

        public class LoginError
        {
            public string ErrorName { get; set; }
            public string ErrorValue { get; set; }
            public LoginError(string name, string value)
            {
                this.ErrorName = name;
                this.ErrorValue = value;
            }
            public static readonly ReadOnlyCollection<LoginError> errors = new ReadOnlyCollection<LoginError>(new LoginError[]
            {
                new LoginError("", ""), //0
                new LoginError("Unknown", "Неизвестная ошибка входа"),
                new LoginError("Blocked", "Пользователь заблокирован"),
                new LoginError("TimeUp", "Время действия ссылки истекло"),
            });
        }

        public static string? CreateJWT<T>(T inputUser, JWT_TYPE type)
        {
            try
            {
                SymmetricSecurityKey secretkey;
                switch (type)
                {
                    case JWT_TYPE.Register:
                    case JWT_TYPE.RegisterResetPass:
                        secretkey = GetSymmetricSecurityRegisterKey();
                        break;

                    case JWT_TYPE.Access:
                        secretkey = GetSymmetricSecurityKey();
                        break;

                    case JWT_TYPE.Refresh:
                        secretkey = GetSymmetricSecurityRefreshKey();
                        break;

                    case JWT_TYPE.WebLogin:
                        secretkey = GetSymmetricSecurityWebLoginKey();
                        break;

                    default:
                        return null;
                        break;
                }

                var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);
                List<Claim> claims = new();
                switch (inputUser)
                {
                    case User:
                        var user = inputUser as User;
                        User userWithoutPass = User.CloneUserWithoutPass(user);
                        claims = new()
                        {
                            //new Claim(ClaimTypes.Name, user.Email),
				            new Claim(JwtRegisteredClaimNames.Sub, type.ToString()),
                            new Claim(JwtRegisteredClaimNames.Email, userWithoutPass.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //token_ID

                            new Claim("user", userWithoutPass.ToJson()),

                        };
                        /*
                        //user уже содержит роли
                        if (user.Roles != null && user.Roles.Count > 0)
                        {
                            foreach (var role in user.Roles)
                            {
                                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
                            }
                        }*/
                        break;

                    case Employee:
                        var employee = inputUser as Employee;
                        Employee employeeWithoutPass = Employee.CloneEmployeeWithoutPass(employee);

                        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, type.ToString()));
                        if (!string.IsNullOrEmpty(employeeWithoutPass.Mail))
                        {
                            claims.Add(new Claim(JwtRegisteredClaimNames.Email, employeeWithoutPass.Mail));
                        }
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); //token_ID

                        claims.Add(new Claim("employee", employeeWithoutPass.ToJson()));

                        break;

                    default:
                        return null;
                        break;
                }


                JwtSecurityToken token = new();
                switch (type)
                {
                    case JWT_TYPE.Register:
                    case JWT_TYPE.RegisterResetPass:
                        token = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(15),
                        signingCredentials: credentials);
                        break;

                    case JWT_TYPE.Access:
                        token = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials);
                        break;

                    case JWT_TYPE.Refresh:
                        //if (!string.IsNullOrEmpty(accessId)) 
                        //{
                        //    claims.Add(new Claim("accessId", accessId));
                        //}

                        token = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.Now.AddMonths(1),
                        signingCredentials: credentials);
                        break;

                    case JWT_TYPE.WebLogin:
                        token = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(1),
                        signingCredentials: credentials);
                        break;

                    default:
                        return null;
                        break;
                }

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuthOptions_CreateJWT_Exception: " + ex.Message);
                return null;
            }
        }

        public static bool ValidateToken(string token, SymmetricSecurityKey key, bool validateIssuer = true, bool validateAudience = true, bool validateLifetime = true)
        {
            var validationParameters = new TokenValidationParameters()
            {
                ValidAudience = AUDIENCE,
                ValidIssuer = ISSUER,
                ValidateIssuer = validateIssuer,
                ValidateAudience = validateAudience,
                ValidateLifetime = validateLifetime,

                IssuerSigningKey = key,
                ValidateIssuerSigningKey = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuthOptions_VerifyToken_Exception: " + ex.Message);
                throw;
            }
            //... ручные проверки возвращают false, если обнаружено что-либо неблагоприятное
            return validatedToken != null;
        }

        public static bool CheckWhiteList(string token)
        {
            try
            {
                JwtWhiteListService service = new(CollectionNames.JwtWhiteList.ToString());
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);

                JwtRow? row = service.GetJwtRowByTokenId(jwtSecurityToken.Id);
                if (row != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuthOptions_CheckWhiteList: " + ex.Message);
                return false;
            }
        }
    }
}
