using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.PurchaseOrderLineModels;
using MES.Models.SalesOrderLineModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers
{
    public class PurchaseOrderLineController : Controller
    {
        readonly ILogger<PurchaseOrderLineController> _logger;
        readonly IHttpClientService _httpClientService;
        readonly IPurchaseOrderLineService _purchaseOrderLineService;
        readonly ICustomQueryService _customQueryService;

        public PurchaseOrderLineController(ILogger<PurchaseOrderLineController> logger,
            IHttpClientService httpClientService,
            IPurchaseOrderLineService purchaseOrderLineService,
            ICustomQueryService customQueryService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _purchaseOrderLineService = purchaseOrderLineService;
            _customQueryService = customQueryService;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Bekleyen Satın Alma İşlemleri";
            return View();
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetSalesOrders() });

        }


        public async IAsyncEnumerable<PurchaseOrderLineListModel> GetSalesOrders()
        {
            PurchaseOrderLineListModel viewModel = new PurchaseOrderLineListModel();
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            if (viewModel != null)
            {
                string query = $@"SELECT
                                ORFLINE.DATE_ AS [OrderDate],
                                ORFLINE.LOGICALREF AS [ReferenceId],
                                ORFLINE.LINEEXP AS [Description],
                                ORFICHE.FICHENO AS [OrderCode],
                                CLCARD.LOGICALREF AS [CurrentReferenceId],
                                CLCARD.CODE as [CurrentCode],
                                CLCARD.DEFINITION_ AS [CurrentName],
                                ITEM.LOGICALREF [ProductReferenceId],
                                ITEM.CODE AS [ProductCode],
                                ITEM.NAME AS [ProductName],
                                CAPIWHOUSE.LOGICALREF AS [WarehouseReferenceId],
                                ISNULL((CAPIWHOUSE.NR),0) AS [WarehouseNo],
                                CAPIWHOUSE.NAME AS [WarehouseName],
                                UNITSET.CODE AS [Unitset],
                                SUBUNITSET.CODE AS [SubUnitset],
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
                                WHERE (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.CLOSED = 0 AND ORFLINE.TRCODE =  2";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<PurchaseOrderLineListModel> result = (List<PurchaseOrderLineListModel>)jsonDocument.Deserialize(typeof(List<PurchaseOrderLineListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (PurchaseOrderLineListModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }
        }
    }
}
