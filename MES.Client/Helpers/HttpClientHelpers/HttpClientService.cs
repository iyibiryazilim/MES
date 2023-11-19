using System;
namespace MES.Client.Helpers.HttpClientHelpers;

public class HttpClientService : IHttpClientService
{
    private readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(
() =>
{
    var httpClient = new HttpClient();
    //httpClient.BaseAddress = new Uri("http://78.189.217.83:1453");
    //httpClient.BaseAddress = new Uri("http://172.25.86.101:16003");
    httpClient.BaseAddress = new Uri("http://195.142.192.18:1089");
	httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

    return httpClient;
}

, LazyThreadSafetyMode.None);
    public string Token { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiaXlpYmlyIiwianRpIjoiMmE2OGNmYTgtNzRhNS00YzFjLTk2ODQtOTYxYzA1NDE5MjMzIiwiZXhwIjoxNzI0MTM3MzU3LCJpc3MiOiJpeWliaXIifQ.SGhKDUHPa4BYYFibj-_2WFXAy3YQ5xi9Khq08eujUaI";

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

