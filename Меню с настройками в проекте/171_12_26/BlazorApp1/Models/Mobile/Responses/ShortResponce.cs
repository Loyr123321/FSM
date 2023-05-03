namespace BlazorApp1.Models.Mobile.Responses
{
    public class ShortResponce
    {
        public bool? Result { get; private set; }
        public ShortResponce(bool result)
        {
            Result = result;
        }
    }
}
