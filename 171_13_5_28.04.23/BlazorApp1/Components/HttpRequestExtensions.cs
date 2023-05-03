namespace BlazorApp1.Components
{
    public static class HttpRequestExtensions
    {
        public static bool IsInternetExplorer(this HttpRequest request)
        {
            return IsInternetExplorer(request.Headers["User-Agent"]);
        }

        private static bool IsInternetExplorer(string userAgent)
        {
            return userAgent.Contains("MSIE")
                   || userAgent.Contains("Trident");
        }
    }
}
