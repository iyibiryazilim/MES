using System;

namespace MES.HttpClientService
{
    public class HttpClientService : IHttpClientService
    {
        private readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(
    () =>
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://195.142.192.18:1087");
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        return httpClient;
    }

    , LazyThreadSafetyMode.None);

        public string Token { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiaXlpYmlyIiwianRpIjoiZTIzMmQzZTUtMTg4ZC00MGY2LTllNTktNDk4MThkNDliOTU1IiwiZXhwIjoxNjgzMjg1MzQ5LCJpc3MiOiJpeWliaXIifQ.dJTpBkn_6EYM976tapKQBCzCW5WUmmVxw_mw_ngTXMg";

        public HttpClient GetOrCreateHttpClient()
        {
            var httpClient = _httpClient.Value;

            if (!string.IsNullOrEmpty(Token))
            {
                if (httpClient.DefaultRequestHeaders.Authorization == null)
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token.Trim('"'));

            }
            else
                httpClient.DefaultRequestHeaders.Authorization = null;

            return httpClient;
        }
    }
}

