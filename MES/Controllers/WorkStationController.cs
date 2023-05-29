using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.Models.WorkstationGroupModels;
using MES.Models.WorkstationModels;
using MES.ViewModels.ProductViewModels;
using MES.ViewModels.WorkStationViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Text.Json;

namespace MES.Controllers
{
    public class WorkStationController : Controller
    {
        readonly ILogger<WorkStationController> _logger;
        readonly IWorkstationServise _service;
        readonly IHttpClientService _httpClientService;
		readonly ICustomQueryService _customQueryService;

		public WorkStationController(ILogger<WorkStationController> logger,
            IHttpClientService httpClientService,
            IWorkstationServise service,ICustomQueryService customQueryService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
			_customQueryService = customQueryService;
        }


        public IActionResult Index()
        {
            ViewData["Title"] = "İş İstasyonları";
            return View();
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetWorkstation() });
        }


        public async IAsyncEnumerable<WorkstationListModel> GetWorkstation()
        {
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			WorkstationListModel viewModel = new();

			if (viewModel != null)
			{
				const string query = @"
						SELECT WORKSTAT.LOGICALREF AS [REFERENCEID],
						WORKSTAT.CODE AS [CODE],
						WORKSTAT.NAME AS [NAME] FROM LG_003_WORKSTAT AS WORKSTAT
				
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

						#region Fill Rate
						viewModel.FillRate = 0;
						#endregion

						#region Estimated Maintenance Date
						viewModel.EstimatedMaintenanceDate = DateTime.Now;
						#endregion


						yield return viewModel;
					}
				}
			}
		}
    }
}
