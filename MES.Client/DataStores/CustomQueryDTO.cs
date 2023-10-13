using System;
using LBS.WebAPI.Service.Services;
using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.Services;

namespace MES.Client.DataStores;

public class CustomQueryDTO : ICustomQueryDTO
{
    ICustomQueryService _customQueryService;
    public CustomQueryDTO(ICustomQueryService customQueryService)
    {
        _customQueryService = customQueryService;
    }
    public async IAsyncEnumerable<dynamic> GetObjectsAsync(IHttpClientService _httpClientService, string query)
    {
        var httpClient = _httpClientService.GetOrCreateHttpClient();
        await foreach (var item in _customQueryService.GetObjects(httpClient, query))
            yield return item;


    }
}

