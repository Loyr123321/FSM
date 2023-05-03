using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BlazorApp1.Services;
using Blazored.Modal;
using MongoDB.Bson.Serialization;
using BlazorApp1.Models;
using PeterLeslieMorris.Blazor.Validation;
using MudBlazor.Services;
using Blazored.Toast;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BlazorApp1.Utils;
using MudBlazor;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BlazorApp1.Auth;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Mvc;
using static BlazorApp1.Pages.Auth.LoginPage;
using System.Reflection;
using MongoDB.Bson.Serialization.Serializers;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BlazorApp1.Models.Mobile.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

UploadPath.CreateDirectories();
RabbitMQService rabbit = new();
await rabbit.CreateDefaultExchange();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme /*JwtBearerDefaults.AuthenticationScheme*/)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Unauthorized/";
        options.AccessDeniedPath = "/Account/Forbidden/";
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
        };


        options.Events = new JwtBearerEvents();
        options.Events.OnChallenge = context =>
        {
            // Skip the default logic.
            context.HandleResponse();
            // Handle expired token error
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 401;

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // Check if token is expired
            if (context.AuthenticateFailure?.GetType() == typeof(SecurityTokenExpiredException))
            {
                string responce = JsonConvert.SerializeObject(new BaseMobileResponce(null, (int)ErrorHandling.ErrorCodes.ERROR_TOKEN_HAS_EXPIRED), settings);
                return context.Response.WriteAsync(responce);
            }

            // Check if token is whitelisted
            var accessToken = context.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var IsWhitelisted = AuthOptions.CheckWhiteList(accessToken);
            if (IsWhitelisted == false)
            {
                string responce = JsonConvert.SerializeObject(new BaseMobileResponce(null, (int)ErrorHandling.ErrorCodes.ERROR_TOKEN_IS_NOT_WHITELISTED), settings);
                return context.Response.WriteAsync(responce);
            }

            return context.Response.WriteAsync(JsonConvert.SerializeObject(new BaseMobileResponce(null, null).ToJson(), settings));
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireRole", policy => policy.RequireRole("Admin", "Dispatcher")); //Require_Role требовать роль
});


// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor();

builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredToast();
builder.Services.AddScoped(x => new JwtWhiteListService(CollectionNames.JwtWhiteList.ToString()));
builder.Services.AddScoped(x => new UserService(CollectionNames.Users.ToString()));
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped(x => new ClientService(CollectionNames.Clients.ToString(), CollectionNames.ClientsHistory.ToString()));
//builder.Services.AddScoped<IValidator<Client>, ClientValidator>();//!!!!!!!!!!!!!
//builder.Services.AddScoped<IValidator<ClientContact>, ClientContactsValidator>();//!!!!!!!!!!!!!
builder.Services.AddScoped(x => new SkillService(CollectionNames.Skills.ToString()));
builder.Services.AddScoped(x => new EmployeeService(CollectionNames.Employees.ToString(), CollectionNames.EmployeesHistory.ToString()));
builder.Services.AddScoped(x => new OrderTypeService(CollectionNames.OrderTypes.ToString()));
builder.Services.AddScoped(x => new DataListService(CollectionNames.DataLists.ToString()));
builder.Services.AddScoped(x => new RegionService(CollectionNames.Regions.ToString()));
builder.Services.AddScoped(x => new CancellationReasonService(CollectionNames.CancellationReasons.ToString()));
//AddDefaultRecords CancellationReason
//CancellationReason cl = new(); cl.AddDefaultRecords(new CancellationReasonService(CollectionNames.CancellationReasons.ToString()));
builder.Services.AddScoped(x => new UserGridSettingsService(CollectionNames.UserGridSettings.ToString()));
builder.Services.AddScoped(x => new GeneralGridSettingsService(CollectionNames.GeneralGridSettings.ToString()));
builder.Services.AddScoped(x => new ReassignmentReasonService(CollectionNames.ReassignmentReasons.ToString()));
builder.Services.AddScoped(x => new OrderService(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString()));
builder.Services.AddScoped(x => new OrderTemplateService(CollectionNames.OrderTemplates.ToString(), CollectionNames.OrderTemplatesHistory.ToString()));
builder.Services.AddScoped<DaDataService>();
builder.Services.AddScoped<RabbitMQService>();

builder.Services.AddScoped(x => new TestService(CollectionNames.Test.ToString()));//09.02.23 Для проверки ДатыВремя

builder.Services.AddHttpContextAccessor(); //HttpContext содержит всю информацию о HTTP-запросе (для получения ip)

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(UploadPath.BaseUrl) });
//builder.Services.AddHttpClient();

builder.WebHost.ConfigureKestrel(k =>
{
    /*
    k.Limits.KeepAliveTimeout = TimeSpan.MaxValue; //время ожидания проверки на активность. Значение по умолчанию — 130 секунд.
    k.Limits.MaxConcurrentConnections = null;
    k.Limits.MaxConcurrentUpgradedConnections = null;
    */
    k.Limits.MaxRequestBodySize = null; //Максимальный размер тела запроса !!!
    /*
    k.Limits.MaxRequestBufferSize = null;
    k.Limits.MaxRequestHeaderCount = int.MaxValue; //максимально допустимое количество заголовков на HTTP-запрос.
    k.Limits.MaxRequestHeadersTotalSize = int.MaxValue; //максимальный допустимый размер заголовков HTTP-запросов
    k.Limits.MaxRequestLineSize = int.MaxValue; //максимальный допустимый размер строки HTTP-запроса.
    */

    /*
    //максимальный размер буфера ответа перед тем, как вызовы записи начинают блокировать или возвращать задачи, которые не завершатся до тех пор,
    //пока размер буфера не будет меньше заданного предела.По умолчанию используется 65 536 байт(64 КБ).
    //Если задано значение NULL, размер буфера ответа не ограничен.
    //Если задано значение нулю, все вызовы записи блокируют или возвращают задачи, которые не завершатся до очистки всего буфера ответа.
    k.Limits.MaxResponseBufferSize = null;
    k.Limits.MinRequestBodyDataRate = null; //минимальную скорость данных текста запроса в байтах в секунду. это ограничение не влияет на обновленные подключения, которые всегда являются неограниченными.
    k.Limits.MinResponseDataRate = null;
    k.Limits.RequestHeadersTimeout = TimeSpan.MaxValue; // максимальное время, затрачивает сервер на получение заголовков запросов
    */
});

//Notification config
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 100;
    config.SnackbarConfiguration.ShowTransitionDuration = 100;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddMudServices();
builder.Services.AddFormValidation(config => config.AddDataAnnotationsValidation());

////BsonSerializer.RegisterSerializer(DateTimeSerializer.UtcInstance);

//Solve Mongo DB Element Does Not Match Any Field or Property of Class (Разрешить несоответствие полей в бд и в моделях)
BsonClassMap.RegisterClassMap<Address>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<Brigade>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<CancellationReason>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<Client>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<Employee>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<Order>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<OrderTemplate>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<OrderType>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<Region>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<Skill>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<User>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});
BsonClassMap.RegisterClassMap<UserGridSettings>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});

BsonClassMap.RegisterClassMap<TextField>(); // do it before you access DB
BsonClassMap.RegisterClassMap<FileField>(); // do it before you access DB
BsonClassMap.RegisterClassMap<PhotoField>(); // do it before you access DB
BsonClassMap.RegisterClassMap<YesNoField>(); // do it before you access DB
BsonClassMap.RegisterClassMap<LinkField>(); // do it before you access DB
BsonClassMap.RegisterClassMap<RubleField>(); // do it before you access DB
BsonClassMap.RegisterClassMap<DoubleField>(); // do it before you access DB
BsonClassMap.RegisterClassMap<LongField>(); // do it before you access DB
BsonClassMap.RegisterClassMap<DateField>(); // do it before you access DB
BsonClassMap.RegisterClassMap<DateTimeField>(); // do it before you access DB
BsonClassMap.RegisterClassMap<ListField>(); // do it before you access DB
//BsonClassMap.RegisterClassMap<MyDictionary>(); // do it before you access DB

BsonClassMap.RegisterClassMap<DataList>(cm =>
{
    cm.AutoMap();
    cm.SetIgnoreExtraElements(true);
});

var app = builder.Build();


app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(new[] { "ru-RU"})
    .AddSupportedUICultures(new[] { "ru-RU" }));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
////-----------
app.UseAuthentication();   // добавление middleware аутентификации 
app.MapControllers();//.AddNewtonsoftJson();
app.UseAuthorization();   // добавление middleware авторизации
////-----------------


app.MapGet("/testlogin", async (HttpContext context) =>
{
    var claimsIdentity = new ClaimsIdentity("Undefined");
    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    // установка аутентификационных куки
    await context.SignInAsync(claimsPrincipal);

    return "Данные установлены";
});


app.MapGet("/loginUser", async (HttpContext context, string token) =>
{
    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var jwtSecurityToken = handler.ReadJwtToken(token);

    var isValid = AuthOptions.ValidateToken(token, AuthOptions.GetSymmetricSecurityWebLoginKey(), true, true);
    if (isValid == false) 
    {
        context.Response.Redirect("/login?errorCode=1", permanent: false);
        return "";
    }

    User? tokenUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(jwtSecurityToken.Claims.First(claim => claim.Type == "user").Value);
    if (tokenUser == null) 
    {
        context.Response.Redirect("/login?errorCode=1", permanent: false);
        return "";
    }

    var claims = new List<Claim>();
    claims.Add(new Claim("UserId", tokenUser.Id, ClaimValueTypes.String));
    claims.Add(new Claim(ClaimTypes.Name, tokenUser.GetFullName()));
    claims.Add(new Claim(ClaimTypes.Email, tokenUser.Email));
    if (tokenUser.Roles != null && tokenUser.Roles.Count > 0)
    {
        foreach (var role in tokenUser.Roles)
        {
            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
        }
    }
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

    await context.SignInAsync(claimsPrincipal);
    context.Response.Redirect("/login", permanent: false);

    return "";
});


app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/", permanent: false);
    return "";
});

app.Map("/data", [Authorize] () => new { message = "Data" });
app.Map("/testadmin", [Authorize(Roles = "Admin")] () => "OnlyAdmin");
app.Map("/testdispatcher", [Authorize(Roles = "Admin,Dispatcher")] () => "Dispatcher");

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();