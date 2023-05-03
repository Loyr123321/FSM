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
        private static readonly string KEY = configuration.GetValue<string>("JsonWebToken:Key"); // ключ для шифрации токенов
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));

        public enum JWT_TYPE
        {
            Access = 0,
            Refresh = 1,
            Register = 2,
            ResetPass = 22,
            WebLogin = 3
        }

        //!!!Не путать Claim_ы токена и ClaimsIdentity это другое

        public static string? CreateRegisterJWT(string loginMail, List<string> roles)
        {
            try
            {
                SymmetricSecurityKey secretkey = GetSymmetricSecurityKey();

                var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);
                List<Claim> claims = new();
                claims = new()
                {
                    new Claim(JwtRegisteredClaimNames.Sub, JWT_TYPE.Register.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //token_ID

                    new Claim(JwtRegisteredClaimNames.Email, loginMail), //Почта(Логин)
                    new Claim("ROLES", roles.ToJson()), //Роли
                };


                JwtSecurityToken token = new();
                token = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(15),
                        signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuthOptions_CreateRegisterJWT_Exception: " + ex.Message);
                return null;
            }
        }
        public static string? CreateResetPassJWT(string loginMail)
        {
            try
            {
                SymmetricSecurityKey secretkey = GetSymmetricSecurityKey();

                var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);
                List<Claim> claims = new();
                claims = new()
                {
                    new Claim(JwtRegisteredClaimNames.Sub, JWT_TYPE.ResetPass.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, loginMail),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //token_ID
                };


                JwtSecurityToken token = new();
                token = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(15),
                        signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuthOptions_CreateResetPassJWT_Exception: " + ex.Message);
                return null;
            }
        }
        public static string? CreateMobileAccessJWT(string userId)
        {
            try
            {
                SymmetricSecurityKey secretkey = GetSymmetricSecurityKey();

                var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);
                List<Claim> claims = new();
                //User userWithoutPass = User.CloneUserWithoutPass(inputUser);
                claims = new()
                {
                    //new Claim(ClaimTypes.Name, user.Email),
				    new Claim(JwtRegisteredClaimNames.Sub, JWT_TYPE.Access.ToString()),
                    //new Claim(JwtRegisteredClaimNames.Email, userWithoutPass.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //token_ID

                    new Claim("id", userId),

                };

                JwtSecurityToken token = new();
                token = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuthOptions_CreateUnityAccessJWT_Exception: " + ex.Message);
                return null;
            }
        }
        public static string? CreateMobileRefreshJWT(string userId)
        {
            try
            {
                SymmetricSecurityKey secretkey = GetSymmetricSecurityKey();

                var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);
                List<Claim> claims = new();
                claims = new()
                {
                    new Claim(JwtRegisteredClaimNames.Sub, JWT_TYPE.Refresh.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //token_ID

                    new Claim("id", userId),
                };


                JwtSecurityToken token = new();
                token = new JwtSecurityToken(
                       issuer: AuthOptions.ISSUER,
                       audience: AuthOptions.AUDIENCE,
                       claims: claims,
                       expires: DateTime.Now.AddMonths(1),
                       signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuthOptions_CreateUnityRefreshJWT_Exception: " + ex.Message);
                return null;
            }
        }
        public static string? CreateWebLoginJWT(User user)
        {
            try
            {
                SymmetricSecurityKey secretkey = GetSymmetricSecurityKey();

                var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);
                List<Claim> claims = new();
                claims = new()
                {
                    new Claim(JwtRegisteredClaimNames.Sub, JWT_TYPE.WebLogin.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //token_ID

                    new Claim("user", user.ToJson()),
                };


                JwtSecurityToken token = new();
                token = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(1),
                        signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("AuthOptions_CreateWebLoginJWT_Exception: " + ex.Message);
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

//The end
    } 
}
