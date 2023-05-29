using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.RouteModels;
using MES.Models.StopCauseModels;
using MES.Models.WorkOrderModels;
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
					  STOPCAUSE.LOGICALREF AS [ReferenceId], 
					  STOPCAUSE.NAME AS [Description], 
					  STOPCAUSE.CODE AS [Code], 
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
                    List<StopCauseListModel> result = (List<StopCauseListModel>)jsonDocument.Deserialize(typeof(List<StopCauseListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (StopCauseListModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }

        }
    }
}
