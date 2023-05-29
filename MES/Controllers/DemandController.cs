using System.Text.Json;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.DemandModels;
using MES.Models.EmployeeGroupModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class DemandController : Controller
    {
        readonly ILogger<DemandController> _logger;
        readonly IDemandService _service;
        readonly IHttpClientService _httpClientService;
        readonly ICustomQueryService _customQueryService;
        public DemandController(ILogger<DemandController> logger,
            IHttpClientService httpClientService,
            IDemandService service, ICustomQueryService customQueryService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _customQueryService = customQueryService;
        }


        public IActionResult Index()
        {
            ViewData["Title"] = "Bekleyen Talepler";
            return View();
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetDemands() });
        }


        public async IAsyncEnumerable<DemandListModel> GetDemands()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            DemandListModel viewModel = new();

            if (viewModel != null)
            {
                const string query = @"

SELECT 
DEMANDFICHE.LOGICALREF AS [ReferenceId],
DEMANDFICHE.FICHENO AS [FicheNo],
DEMANDFICHE.DATE_ AS [Date],
DEMANDFICHE.STATUS AS [Status],
DEMANDFICHE.SOURCEINDEX AS [WarehouseNo],
ITEMS.LOGICALREF AS [ProductReferenceId],
ITEMS.CODE AS [ProductCode],
ITEMS.NAME AS[ProductName],
DEMANDLINE.LOGICALREF [DemandLineReferenceId],
DEMANDLINE.AMOUNT AS [DemandLineAmounth],
DEMANDLINE.MEETAMNT AS [DemandLineMeetAmounth],
DEMANDLINE.CANCAMOUNT AS [DemandLineCancelAmounth],
SUBUNITSET.CODE AS [SubunitsetCode],
UNITSET.CODE AS [UnitsetCode]
FROM LG_003_01_DEMANDLINE AS DEMANDLINE
LEFT JOIN LG_003_01_DEMANDFICHE AS DEMANDFICHE ON DEMANDFICHE.LOGICALREF = DEMANDLINE.DEMANDFICHEREF
LEFT JOIN LG_003_ITEMS AS ITEMS ON ITEMS.LOGICALREF = DEMANDLINE.ITEMREF
LEFT JOIN LG_003_UNITSETF AS UNITSET ON DEMANDLINE.USREF = UNITSET.LOGICALREF
LEFT JOIN LG_003_UNITSETL AS SUBUNITSET ON DEMANDLINE.UOMREF = SUBUNITSET.LOGICALREF


";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<DemandListModel> result = (List<DemandListModel>)jsonDocument.Deserialize(typeof(List<DemandListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (DemandListModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }
        }
    }
}
