using System.Text.Json;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.RouteModels;
using MES.Models.SalesOrderLineModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class RouteController : Controller
    {
        readonly ILogger<RouteController> _logger;
        readonly IHttpClientService _httpClientService;
        readonly IRouteService _service;
        readonly ICustomQueryService _customQueryService;

        public RouteController(ILogger<RouteController> logger,
            IHttpClientService httpClientService,
            IRouteService service,
            ICustomQueryService customQueryService
            )
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _customQueryService = customQueryService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Rotalar";
            return View();
        }

        [HttpPost]
        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetOperation() });
        }

        public async IAsyncEnumerable<RouteListModel> GetOperation()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            RouteListModel viewModel = new();

            if (viewModel != null)
            {
                const string query = @"SELECT 
					  ROUTING.LOGICALREF AS [ReferenceId], 
					  ROUTING.CODE AS [Code], 
					  ROUTING.WFSTATUS AS [Status], 
					  ROUTING.NAME AS [Description], 
					  ROUTING.CARDTYPE AS [CardType] 
					FROM 
					  LG_003_ROUTING AS ROUTING";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<RouteListModel> result = (List<RouteListModel>)jsonDocument.Deserialize(typeof(List<RouteListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (RouteListModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }

        }


    }
}
