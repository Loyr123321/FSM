namespace BlazorApp1.Models.Mobile.Responses
{
    public class ShortResponse
    {
        public bool? Result { get; private set; }
        public ShortResponse(bool result)
        {
            Result = result;
        }
    }
}
