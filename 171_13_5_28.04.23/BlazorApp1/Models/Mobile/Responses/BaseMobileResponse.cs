using BlazorApp1.Utils;

namespace BlazorApp1.Models.Mobile.Responses
{
    public class BaseMobileResponse
    {
        public object Data { get; set; }
        public int ErrorCode { get; set; } = 0;
        public string ErrorMessage { get; set; } = string.Empty;
        public BaseMobileResponse(object data, int errorCode)
        {
            Data = data;
            ErrorCode = errorCode;
            ErrorMessage = ErrorHandling.GetErrorMessage(errorCode);
        }

        //Unknown error
        public BaseMobileResponse(object data, string errorMessage)
        {
            Data = data;
            ErrorCode = (int)ErrorHandling.ErrorCodes.ERROR_UNKNOWN;
            ErrorMessage = errorMessage;
        }
    }
}
