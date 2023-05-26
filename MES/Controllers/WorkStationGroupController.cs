using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.StopCauseModels;
using MES.Models.WorkstationGroupModels;
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
		IWorkstationGroupService workstationGroupService,ICustomQueryService customQueryService)
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
                var array = jsonDocument.RootElement.EnumerateArray();
                foreach (JsonElement element in array)
                {
                    #region Reference Id
                    JsonElement referenceId = element.GetProperty("referenceid");
                    viewModel.ReferenceId = Convert.ToInt32(referenceId.GetRawText().Replace('.', ','));
                    #endregion

                    #region Code
                    JsonElement code = element.GetProperty("code");
                    viewModel.Code = code.GetString();
                    #endregion

                    #region Description
                    JsonElement name = element.GetProperty("name");
                    viewModel.Name = name.GetString();
                    #endregion

                    #region Workstation Count
                    JsonElement workstationCount = element.GetProperty("workstationCount");
                    viewModel.WorkstationCount = Convert.ToInt32(workstationCount.GetRawText().Replace('.', ','));
                    #endregion
                    yield return viewModel;
                }
            }
        }

    }
}
