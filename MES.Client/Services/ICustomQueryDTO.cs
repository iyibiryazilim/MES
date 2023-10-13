using System;
using MES.Client.Helpers.HttpClientHelpers;

namespace MES.Client.Services;

public interface ICustomQueryDTO
{
    public IAsyncEnumerable<dynamic> GetObjectsAsync(IHttpClientService _httpClientService, string query);

}

