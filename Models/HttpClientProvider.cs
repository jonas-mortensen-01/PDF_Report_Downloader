public static class HttpClientProvider
{
    public static readonly HttpClient _sharedClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10)
    };
}