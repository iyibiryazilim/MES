using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.StopCauseModels;
using MES.Models.WorkstationGroupModels;
using MES.Models.WorkstationModels;
using MES.ViewModels.WorkStationGroupViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers;

public class WorkStationGroupController : Controller
{
    readonly ILogger<WorkStationGroupController> _logger;
    readonly IHttpClientService _httpClientService;
    readonly IWorkstationGroupService _workstationGroupService;
    readonly ICustomQueryService _customQueryService;
    public WorkStationGroupController(ILogger<WorkStationGroupController> logger,
        IHttpClientService httpClientService,
        IWorkstationGroupService workstationGroupService, ICustomQueryService customQueryService)
    {
        _logger = logger;
        _httpClientService = httpClientService;
        _workstationGroupService = workstationGroupService;
        _customQueryService = customQueryService;
    }
    public IActionResult Index()
    {
        ViewData["Title"] = "İş İstasyonu Grupları";
        return View();
    }

    [HttpPost]
    public async ValueTask<IActionResult> GetJsonResult()
    {
        return Json(new { data = GetWorkstations() });
    }

    private async IAsyncEnumerable<WorkstationGroupListModel> GetWorkstations()
    {
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        WorkstationGroupListModel viewModel = new();

        if (viewModel != null)
        {
            const string query = @"SELECT 
  WSGROUP.CODE AS [CODE], 
  WSGROUP.LOGICALREF AS [REFERENCEID], 
  WSGROUP.NAME AS [NAME], 
  [WorkstationCount] = (
    SELECT 
      COUNT(WSGRPREF) 
    FROM 
      LG_003_WSGRPASS 
    WHERE 
      WSGRPREF = WSGROUP.LOGICALREF
  ) 
FROM 
  LG_003_WSGRPF AS WSGROUP

";
            JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
            if (jsonDocument != null)
            {
                List<WorkstationGroupListModel> result = (List<WorkstationGroupListModel>)jsonDocument.Deserialize(typeof(List<WorkstationGroupListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (WorkstationGroupListModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }

    }
}
