using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.EmployeeGroupModels;
using MES.Models.WorkstationGroupModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers
{
    public class EmployeeGroupController : Controller
	{
		ILogger<EmployeeGroupController> _logger;
		IHttpClientService _httpClientService;
		IEmployeeGroupService _service;
        ICustomQueryService _customQueryService;

        public EmployeeGroupController(ILogger<EmployeeGroupController> logger,
			IHttpClientService httpClientService,
			IEmployeeGroupService service,ICustomQueryService customQueryService)
        {
            _logger = logger;
			_httpClientService = httpClientService;
			_service = service;
            _customQueryService = customQueryService;

        }

        public IActionResult Index()
		{
            ViewData["Title"] = "Çalışan Grupları";
            return View();
		}

		[HttpPost]
		public async ValueTask<IActionResult> GetJsonResult()
		{
			return Json(new { data = GetEmployeGroups() });
		}

		public async IAsyncEnumerable<EmployeeGroupListModel> GetEmployeGroups()
		{
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            EmployeeGroupListModel viewModel = new();

            if (viewModel != null)
            {
                const string query = @"SELECT 
  EMPGROUP.LOGICALREF AS [REFERENCEID], 
  EMPGROUP.CODE AS [CODE], 
  EMPGROUP.NAME AS [NAME], 
  [EmployeeCount] = (
    SELECT 
      COUNT(EMPGRPREF) 
    FROM 
      LG_003_EMGRPASS 
    WHERE 
      EMPGRPREF = EMPGROUP.LOGICALREF
  ) 
FROM 
  LG_003_EMPGROUP AS EMPGROUP


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
                        JsonElement employeeCount = element.GetProperty("employeeCount");
                        viewModel.EmployeeCount = Convert.ToInt32(employeeCount.GetRawText().Replace('.', ','));
                        #endregion
                        yield return viewModel;
                    }
                }
            }
        }
	}
}
