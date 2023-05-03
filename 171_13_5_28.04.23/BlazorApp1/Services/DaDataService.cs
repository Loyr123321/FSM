using DaData;
using DaData.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlazorApp1.Services
{
    public class DaDataService
    {
        public ApiClient client { get; set; }
        public DaDataService()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string dadataSecret = configuration.GetValue<string>("DaDataService:DaDataSecret");
            string dadataToken = configuration.GetValue<string>("DaDataService:DaDataToken");

            client = new ApiClient(new ApiClientOptions()
            {
                LimitQueries = int.MaxValue,
                Secret = dadataSecret,
                Token = dadataToken
            });
        }

        public async Task<DaData.Models.Suggestions.Responses.AddressResponse> GetAddress(string query)
        {
            //Почему подсказки не возвращают площадь квартиры, ближайшее метро и часовой пояс?
            //Эти поля возвращаются только для тарифа «Максимальный».
            return await client.SuggestionsQueryAddress(query, 10);
        }
    }
}
