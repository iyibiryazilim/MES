using System;
namespace MES.HttpClientService
{
	public interface IHttpClientService
	{
        HttpClient GetOrCreateHttpClient();
        string Token { get; set; }
    }
}

