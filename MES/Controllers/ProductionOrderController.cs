using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.ProductWarehouseParameterModels;
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
        IHttpClientService httpClientService, IProductionService productionService,ICustomQueryService customQueryService)
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
              PRODORD.LOGICALREF AS [REFERENCEID], 
              PRODORD.FICHENO AS [CODE], 
              PRODORD.GENEXP1 AS [DESCRIPTION], 
              PRODORD.PLNBEGDATE AS [PLANNEDBEGDATE],
              ITEM.LOGICALREF AS [PRODUCTREFERENCEID],  
              ITEM.CODE AS [PRODUCTCODE], 
              ITEM.NAME AS [PRODUCTNAME], 
              SUBUNITSET.CODE AS [SUBUNITSET], 
              UNITSET.CODE AS [UNITSET], 
              PRODORD.PLNAMOUNT AS [PLANNEDAMOUNTH], 
              PRODORD.ACTAMOUNT AS [ACTUALAMOUNTH] 
            FROM 
              LG_003_PRODORD AS PRODORD 
              LEFT JOIN LG_003_ITEMS AS ITEM ON PRODORD.ITEMREF = ITEM.LOGICALREF 
              LEFT JOIN LG_003_UNITSETF AS UNITSET ON PRODORD.UOMREF = UNITSET.LOGICALREF 
              LEFT JOIN LG_003_UNITSETL AS SUBUNITSET ON PRODORD.USETREF = SUBUNITSET.LOGICALREF";

            JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
            if (jsonDocument != null)
            {
                var array = jsonDocument.RootElement.EnumerateArray();
                foreach (JsonElement element in array)
                {
                    #region Reference Id
                    JsonElement referenceId = element.GetProperty("referenceid");
                    viewModel.ReferenceId = referenceId.GetInt32();
                    #endregion

                    #region Code
                    JsonElement code = element.GetProperty("code");
                    viewModel.Code = code.GetString();
                    #endregion

                    #region Description
                    JsonElement description = element.GetProperty("description");
                    viewModel.Description = description.GetString();
                    #endregion

                    #region Planned Begin Date
                    JsonElement plannedBeginDate = element.GetProperty("plannedbegdate");
                    if (element.TryGetProperty("date", out plannedBeginDate) && plannedBeginDate.ValueKind != JsonValueKind.Null)
                    {
                        viewModel.PlannedBeginDate = JsonSerializer.Deserialize<DateTime>(plannedBeginDate.GetRawText());
                    }
                    #endregion                  

                    #region Product Reference Id
                    JsonElement productReferenceId = element.GetProperty("productreferenceid");
                    viewModel.ProductRefernceId = productReferenceId.GetInt32();
                    #endregion

                    #region Product Code
                    JsonElement productCode = element.GetProperty("productcode");
                    viewModel.ProductCode = productCode.GetString();
                    #endregion

                    #region Product Name 
                    JsonElement productName = element.GetProperty("productname");
                    viewModel.ProductName = productName.GetString();
                    #endregion

                    #region Unitset
                    JsonElement unitset = element.GetProperty("unitset");
                    viewModel.Unitset = unitset.GetString();
                    #endregion

                    #region SubUnitSet
                    JsonElement subUnitSet = element.GetProperty("subunitset");
                    viewModel.SubUnitset = subUnitSet.GetString();
                    #endregion

                    #region Planned Amounth
                    JsonElement plannedAmounth = element.GetProperty("plannedamounth");
                    viewModel.PlannedAmounth = Convert.ToDouble(plannedAmounth.GetRawText().Replace('.', ','));
                    #endregion

                    #region Planned Amounth
                    JsonElement actualAmounth = element.GetProperty("actualamounth");
                    viewModel.ActualAmounth = Convert.ToDouble(actualAmounth.GetRawText().Replace('.', ','));
                    #endregion

                    #region Realization Rate
					viewModel.RealizationRate = Convert.ToInt64(100*(viewModel.ActualAmounth / viewModel.PlannedAmounth));
                    #endregion

                    yield return viewModel;
                }
            }
        }


    }

}
