using System.Text.Json;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.ViewModels.RawProductViewModel;
using MES.ViewModels.SalesOrderLine;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class SalesOrderLineController : Controller
    {
        readonly ILogger<SalesOrderLineController> _logger;
        readonly IHttpClientService _httpClientService;
        readonly ISalesOrderLineService _salesOrderLineService;
        readonly ICustomQueryService _customQueryService;

        public SalesOrderLineController(ILogger<SalesOrderLineController> logger,
             IHttpClientService httpClientService,
             ISalesOrderLineService salesOrderLineService, ICustomQueryService customQueryService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _salesOrderLineService = salesOrderLineService;
            _customQueryService = customQueryService;

        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Bekleyen Satış Siparişleri";

            return View();
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetSalesOrders() });

        }


        public async IAsyncEnumerable<SalesOrderLineViewModel> GetSalesOrders()
        {
            SalesOrderLineViewModel viewModel = new SalesOrderLineViewModel();
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            if (viewModel != null)
            {
                string query = $@"SELECT
                                ORFLINE.DATE_ AS [DATE],
                                ORFLINE.LOGICALREF AS [ORDERREFERENCEID],
                                ORFLINE.LINEEXP AS [DESCRIPTION],
                                ORFICHE.FICHENO AS [FICHENO],
                                CLCARD.LOGICALREF AS [CURRENTREFERENCEID],
                                CLCARD.CODE as [CURRENTCODE],
                                CLCARD.DEFINITION_ AS [CURRENTNAME],
                                ITEM.LOGICALREF [PRODUCTREFERENCEID],
                                ITEM.CODE AS [PRODUCTCODE],
                                ITEM.NAME AS [PRODUCTNAME],
                                CAPIWHOUSE.LOGICALREF AS [WAREHOUSEREFERENCEID],
                                ISNULL((CAPIWHOUSE.NR),0) AS [WAREHOUSENO],
                                CAPIWHOUSE.NAME AS [WAREHOUSENAME],
                                UNITSET.CODE AS [UNITSET],
                                SUBUNITSET.CODE AS [SUBUNITSET],
                                [Quantity] = ORFLINE.AMOUNT,
                                [ShippedQuantity] = ORFLINE.SHIPPEDAMOUNT,
                                [WaitingQuantity] = ISNULL((ORFLINE.AMOUNT-ORFLINE.SHIPPEDAMOUNT),0)
                                FROM LG_003_01_ORFLINE AS ORFLINE
                                LEFT JOIN LG_003_01_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
                                LEFT JOIN LG_003_ITEMS AS ITEM ON ORFLINE.STOCKREF = ITEM.LOGICALREF
                                LEFT JOIN LG_003_UNITSETF AS UNITSET ON ORFLINE.UOMREF = UNITSET.LOGICALREF
                                LEFT JOIN LG_003_UNITSETL AS SUBUNITSET ON ORFLINE.USREF = SUBUNITSET.LOGICALREF
                                LEFT JOIN LG_003_CLCARD AS CLCARD ON ORFLINE.CLIENTREF = CLCARD.LOGICALREF
                                LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON ORFLINE.SOURCEINDEX = CAPIWHOUSE.LOGICALREF
                                WHERE (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.CLOSED = 0 AND ORFLINE.TRCODE =  1";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    var array = jsonDocument.RootElement.EnumerateArray();
                    foreach (JsonElement element in array)
                    {
                        #region Reference Id
                        JsonElement referenceId = element.GetProperty("orderreferenceid");
                        viewModel.ReferenceId = referenceId.GetInt32();
                        #endregion

                        #region Current Reference Id
                        JsonElement currentReferenceId = element.GetProperty("currentreferenceid");
                        viewModel.CurrentReferenceId = currentReferenceId.GetInt32();
                        #endregion

                        #region Current Code
                        JsonElement currentCode = element.GetProperty("currentcode");
                        viewModel.CurrentCode = currentCode.GetString();
                        #endregion

                        #region Current Name
                        JsonElement currentName = element.GetProperty("currentname");
                        viewModel.CurrentName = currentName.GetString();
                        #endregion

                        #region Product Reference Id
                        JsonElement productReferenceId = element.GetProperty("productreferenceid");
                        viewModel.ProductReferenceId = productReferenceId.GetInt32();
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

                        #region Quantity
                        JsonElement quantity = element.GetProperty("quantity");
                        viewModel.Quantity = Convert.ToDouble(quantity.GetRawText().Replace('.', ','));
                        #endregion

                        #region Shipped Quantity
                        JsonElement shippedQuantity = element.GetProperty("shippedQuantity");
                        viewModel.ShippedQuantity = Convert.ToDouble(shippedQuantity.GetRawText().Replace('.', ','));
                        #endregion

                        #region Waiting Quantity
                        JsonElement waitingQuantity = element.GetProperty("waitingQuantity");
                        viewModel.WaitingQuantity = Convert.ToDouble(waitingQuantity.GetRawText().Replace('.', ','));
                        #endregion

                        #region Warehouse No
                        JsonElement warehouseNo = element.GetProperty("warehouseno");
                        viewModel.WarehouseNo = warehouseNo.GetInt32();
                        #endregion

                        #region Warehouse Name
                        JsonElement warehouseName = element.GetProperty("warehousename");
                        viewModel.WarehouseName = warehouseName.GetString();
                        #endregion

                        #region Order Code
                        JsonElement orderCode = element.GetProperty("ficheno");
                        viewModel.OrderCode = orderCode.GetString();
                        #endregion

                        #region Description
                        JsonElement description = element.GetProperty("description");
                        viewModel.Description = description.GetString();
                        #endregion

                        #region LastTransactionDate
                        JsonElement orderDate = element.GetProperty("date");

                        if (element.TryGetProperty("date", out orderDate) && orderDate.ValueKind != JsonValueKind.Null)
                        {
                            viewModel.OrderDate = JsonSerializer.Deserialize<DateTime>(orderDate.GetRawText());

                        }
                        #endregion

                        yield return viewModel;
                    }
                }
            }
        }

    }
}
