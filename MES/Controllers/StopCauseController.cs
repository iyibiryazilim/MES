using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.RouteModels;
using MES.Models.StopCauseModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers
{
    public class StopCauseController : Controller
    {
        readonly ILogger<StopCauseController> _logger;
        readonly IStopCauseService _stopCauseService;
        readonly IHttpClientService _httpClientService;
		readonly ICustomQueryService _customQueryService;

		public StopCauseController(ILogger<StopCauseController> logger,
            IStopCauseService stopCauseService,
            IHttpClientService httpClientService, ICustomQueryService customQueryService)
        {
            _logger = logger;
            _stopCauseService = stopCauseService;
            _httpClientService = httpClientService;
			_customQueryService = customQueryService;


		}

        public IActionResult Index()
        {
            ViewData["Title"] = "Durma Nedenleri";
            return View();
        }

        [HttpPost]
        public async ValueTask<IActionResult> GetJsonResult() 
        {
            return Json(new { data = GetStopCauses() });
        }

        public async IAsyncEnumerable<StopCauseListModel> GetStopCauses()
        {
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			StopCauseListModel viewModel = new();

			if (viewModel != null)
			{
				const string query = @"
									SELECT 
					  STOPCAUSE.LOGICALREF AS [REFERENCEID], 
					  STOPCAUSE.NAME AS [DESCRIPTION], 
					  STOPCAUSE.CODE AS [CODE], 
					  [StopDuration] = (
					    SELECT 
					      ISNULL(
					        SUM(STOPDURATION), 
					        0
					      ) 
					    FROM 
					      LG_003_STOPTRANS 
					    WHERE 
					      CAUSEREF = STOPCAUSE.LOGICALREF
					  ), 
					  [StopCauseCount] = (
					    SELECT 
					      COUNT(DISTINCT CAUSEREF) 
					    FROM 
					      LG_003_STOPTRANS 
					    WHERE 
					      CAUSEREF = STOPCAUSE.LOGICALREF
					  ) 
					FROM 
					  LG_003_STOPCAUSE AS STOPCAUSE
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
						JsonElement description = element.GetProperty("description");
						viewModel.Description = description.GetString();
						#endregion

						#region Stop Duration
						JsonElement stopDuration = element.GetProperty("stopDuration");
						viewModel.StopDuration = Convert.ToDouble(stopDuration.GetRawText().Replace('.', ','));
						#endregion

						#region Stop Count
						JsonElement stopCauseCount = element.GetProperty("stopCauseCount");
						viewModel.StopCount = Convert.ToInt32(referenceId.GetRawText().Replace('.', ','));
						#endregion
						yield return viewModel;
					}
				}
			}

		}
    }
}
