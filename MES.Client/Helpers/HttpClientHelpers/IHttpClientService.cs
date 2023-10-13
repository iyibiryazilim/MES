using System;
namespace MES.Client.Helpers.HttpClientHelpers;

public interface IHttpClientService
{
    HttpClient GetOrCreateHttpClient();
    string Token { get; set; }
}

