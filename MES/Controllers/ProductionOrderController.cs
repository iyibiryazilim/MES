using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.ProductionOrderModels;
using MES.Models.SalesOrderLineModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers;

public class ProductionOrderController : Controller
{

    readonly ILogger<ProductionOrderController> _logger;
    readonly IHttpClientService _httpClientService;
    readonly IProductionService _productionService;
    readonly ICustomQueryService _customQueryService;
    public ProductionOrderController(ILogger<ProductionOrderController> logger,
        IHttpClientService httpClientService, IProductionService productionService, ICustomQueryService customQueryService)
    {
        _logger = logger;
        _httpClientService = httpClientService;
        _productionService = productionService;
        _customQueryService = customQueryService;
    }
    public IActionResult Index()
    {
        ViewData["Title"] = "Üretim Emirleri";
        return View();
    }

    [HttpPost]
    public async ValueTask<IActionResult> GetJsonResult()
    {
        return Json(new { data = GetProductions() });
    }

    private async IAsyncEnumerable<ProductionOrderListModel> GetProductions()
    {
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        ProductionOrderListModel viewModel = new();
        if (viewModel != null)
        {
            const string query = @"
        SELECT 
              PRODORD.LOGICALREF AS [ReferenceId], 
              PRODORD.FICHENO AS [Code], 
              PRODORD.GENEXP1 AS [Description], 
              PRODORD.PLNBEGDATE AS [PlannedBeginDate],
              ITEM.LOGICALREF AS [ProductReferenceId],  
              ITEM.CODE AS [ProductCode], 
              ITEM.NAME AS [ProductName], 
              SUBUNITSET.CODE AS [SubUnitset], 
              UNITSET.CODE AS [Unitset], 
              PRODORD.PLNAMOUNT AS [PlannedAmounth], 
              PRODORD.ACTAMOUNT AS [ActualAmounth] 
            FROM 
              LG_003_PRODORD AS PRODORD 
              LEFT JOIN LG_003_ITEMS AS ITEM ON PRODORD.ITEMREF = ITEM.LOGICALREF 
              LEFT JOIN LG_003_UNITSETF AS UNITSET ON PRODORD.UOMREF = UNITSET.LOGICALREF 
              LEFT JOIN LG_003_UNITSETL AS SUBUNITSET ON PRODORD.USETREF = SUBUNITSET.LOGICALREF";

            JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
            if (jsonDocument != null)
            {
                List<ProductionOrderListModel> result = (List<ProductionOrderListModel>)jsonDocument.Deserialize(typeof(List<ProductionOrderListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (ProductionOrderListModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }


    }

}
