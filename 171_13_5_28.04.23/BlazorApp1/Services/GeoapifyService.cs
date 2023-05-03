using BlazorApp1.Models;
using Newtonsoft.Json.Linq;

namespace BlazorApp1.Services
{
    public class GeoapifyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _geoapifyToken;
        public GeoapifyService(HttpClient httpClient)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _geoapifyToken = configuration.GetValue<string>("GeoapifyService:GeoapifyToken");

            _httpClient = httpClient;
        }

        public async Task<string> GetTimeZone(string lat, string lon)
        {
            var response = await _httpClient.GetAsync($"https://api.geoapify.com/v1/geocode/reverse?lat={lat}&lon={lon}&apiKey={_geoapifyToken}");

            using (var content = response.Content)
            {
                var json = await content.ReadAsStringAsync();

                try
                {
                    var data = JObject.Parse(json);
                    var timeZoneName = data["features"]?[0]?["properties"]?["timezone"]?["name"]?.ToString();
                    //var timeZoneOffset = data["features"][0]["properties"]["timezone"]["offset_DST"].ToString();
                    //var tt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, timeZoneName); пример перевода в др часовой пояс

                    return timeZoneName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return string.Empty;
                }

                //return string.Empty;
            }
        }
    }
}
