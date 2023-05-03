namespace BlazorApp1.Models.Mobile.Responses
{
    public class EmployeeAuthResponse
    {
        public string? Id { get; private set; }
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }


        public EmployeeAuthResponse(string? id, string? accessToken, string? refreshToken)
        {
            Id = id;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
