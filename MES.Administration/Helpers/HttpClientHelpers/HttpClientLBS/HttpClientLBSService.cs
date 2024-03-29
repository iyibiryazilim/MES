﻿using System;
namespace MES.Administration.Helpers.HttpClientHelpers.HttpClientLBS;

public class HttpClientLBSService : IHttpClientLBSService
{
    private readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(
() =>
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("http://195.142.192.18:1089");
    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

    return httpClient;
}

, LazyThreadSafetyMode.None);

    public string Token { get; set; }

    public HttpClient GetOrCreateHttpClient()
    {
        var httpClient = _httpClient.Value;

        //if (!string.IsNullOrEmpty(Token))
        //{
        //    if (httpClient.DefaultRequestHeaders.Authorization == null)
        //        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token.Trim('"'));

        //}
        //else
        //    httpClient.DefaultRequestHeaders.Authorization = null;

        return httpClient;
    }
}

