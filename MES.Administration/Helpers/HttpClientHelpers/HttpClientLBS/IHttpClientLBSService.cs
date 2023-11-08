using System;
namespace MES.Administration.Helpers.HttpClientHelpers.HttpClientLBS;

public interface IHttpClientLBSService
{
    HttpClient GetOrCreateHttpClient();
    string Token { get; set; }
}

