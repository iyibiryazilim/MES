using AutoMapper;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.ProductModels.EndProductModels;
using MES.Models.ProductModels.RawProductModels;
using MES.ViewModels.ProductViewModels.RawProductViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers
{
    public class RawProductController : Controller
    {
        readonly ILogger<RawProductController> _logger;
        readonly IRawProductService _service;
        readonly IHttpClientService _httpClientService;
        readonly IMapper _mapper;
        readonly IProductTransactionLineService _transactionLineService;
        readonly IWarehouseTotalService _warehouseTotalService;
        readonly IProductWarehouseParameterService _warehouseParameterService;
        readonly IProductMeasureService _productMeasureService;
        readonly ICustomQueryService _customQueryService;
        readonly ISalesOrderLineService _salesOrderLineService;
        readonly IPurchaseOrderLineService _purchaseOrderLineService;

        public RawProductController(ILogger<RawProductController> logger,
            IHttpClientService httpClientService,
            IRawProductService service,
            IMapper mapper,
            IProductTransactionLineService transactionLineService,
            IWarehouseTotalService warehouseTotalService,
            IProductMeasureService productMeasureService,
            IProductWarehouseParameterService warehouseParameterService,
            ICustomQueryService customQueryService,
            ISalesOrderLineService salesOrderLineService,
            IPurchaseOrderLineService purchaseOrderLineService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _mapper = mapper;
            _transactionLineService = transactionLineService;
            _warehouseTotalService = warehouseTotalService;
            _productMeasureService = productMeasureService;
            _warehouseParameterService = warehouseParameterService;
            _customQueryService = customQueryService;
            _salesOrderLineService = salesOrderLineService;
            _purchaseOrderLineService = purchaseOrderLineService;

        }


        public IActionResult Index()
        {
            ViewData["Title"] = "Hammaddeler";
            return View();
        }
        public async Task<IActionResult> Detail(int productReferenceId)
        {
			ViewData["Header"] = "Hammadde Detay";
			RawProductDetailViewModel viewModel = new RawProductDetailViewModel();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			if (httpClient == null)
				return BadRequest();
			else
			{
				var product = await _service.GetObject(httpClient, productReferenceId);

				if (product == null)
					return NotFound();


				viewModel.RawProductModel = _mapper.Map<RawProductModel>(product);
				viewModel.RawProductModel.OutputQuantity = 0;
				viewModel.RawProductModel.StockQuantity = 0;
				viewModel.RawProductModel.FirstQuantity = 0;
				viewModel.RawProductModel.InputQuantity = 0;
				viewModel.RawProductModel.RevolutionSpeed = 0;

                
            await foreach (RawProductMeasureModel model in GetProductMesaure(productReferenceId))
                viewModel.RawProductMeasureModel.Add(model);



			}

			return View(viewModel);
		}


        public async ValueTask<IActionResult> GetRawProductJsonResult()
        {
            return Json(new { data = GetRawProduct() });
        }
        public async ValueTask<IActionResult> GetInputJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetInputRawProduct(productReferenceId) });
        }
        public async ValueTask<IActionResult> GetOutputJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetOutputRawProduct(productReferenceId) });
        }
        public async ValueTask<IActionResult> GetSalesOrderLineJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetRawProductBySalesOrderLine(productReferenceId) });
        }
        public async ValueTask<IActionResult> GetPurchaseOrderLineJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetRawProductByPurchaseOrderLine(productReferenceId) });
        }

        public async ValueTask<IActionResult> GetWarehouseTotalJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetWarehouseTotalRawProduct(productReferenceId) });
        }

        public async ValueTask<IActionResult> GetProductMesaureJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetProductMesaure(productReferenceId) });
        }
        public async ValueTask<IActionResult> GetWarehouseParameterJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetWarehouseParameterEndProduct(productReferenceId) });
        }


        public async IAsyncEnumerable<RawProductListModel> GetRawProduct()
        {
            RawProductListModel viewModel = new RawProductListModel();
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            if (viewModel != null)
            {
                string query = $@"
SELECT 
  ITEM.LOGICALREF AS [ReferenceId], 
  ITEM.CODE AS [Code], 
  ITEM.NAME AS [Name], 
  ITEM.PRODUCERCODE AS [ProducerCode], 
  ITEM.SPECODE AS [SpeCode], 
  UNITSET.CODE AS [Unitset], 
  [StockQuantity] = ISNULL(
    (
      SELECT 
        SUM(ONHAND) 
      FROM 
        LV_003_01_STINVTOT 
      WHERE 
        STOCKREF = ITEM.LOGICALREF 
        AND INVENNO = -1
    ), 
    0
  ), 
  [InputQuantity] = ISNULL(
    (
      SELECT 
        SUM(AMOUNT) 
      FROM 
        LG_003_01_STLINE 
      WHERE 
        STOCKREF = ITEM.LOGICALREF 
        AND IOCODE IN(1, 2)
    ), 
    0
  ), 
  [OutputQuantity] = ISNULL(
    (
      SELECT 
        SUM(AMOUNT) 
      FROM 
        LG_003_01_STLINE 
      WHERE 
        STOCKREF = ITEM.LOGICALREF 
        AND IOCODE IN(3, 4)
    ), 
    0
  ), 
  [LastTransactionDate] =(
    SELECT 
      TOP 1 DATE_ 
    FROM 
      LG_003_01_STLINE 
    WHERE 
      STOCKREF = ITEM.LOGICALREF 
    ORDER BY 
      DATE_ DESC
  ) 
FROM 
  LG_003_ITEMS AS ITEM 
  LEFT JOIN LG_003_UNITSETF AS UNITSET ON ITEM.UNITSETREF = UNITSET.LOGICALREF 
WHERE 
  ITEM.CARDTYPE = 10


";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<RawProductListModel> result = (List<RawProductListModel>)jsonDocument.Deserialize(typeof(List<RawProductListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (RawProductListModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }

        }

        public async IAsyncEnumerable<RawProductInputTransactionModel> GetInputRawProduct(int productReferenceId)
        {
			RawProductInputTransactionModel viewModel = new RawProductInputTransactionModel();
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			if (viewModel != null)
			{
				string query = $@"SELECT 
			STLINE.LOGICALREF AS [ReferenceId],
			STLINE.DATE_ AS [TransactionDate],
			ISNULL(STLINE.AMOUNT,0) AS [Quantity],
			STLINE.LINEEXP AS [Description],
			UNITSET.CODE AS [UnitsetCode],
			STFICHE.LOGICALREF AS [ProductTransactionReferenceId],
			STFICHE.FICHENO AS [TransactionCode],
			STFICHE.TRCODE AS [TransactionType],
			CAPIWHOUSE.LOGICALREF AS [WarehouseReferenceId],
			CAPIWHOUSE.NR AS [WarehouseNumber],
			CAPIWHOUSE.NAME AS [WarehouseName]
			FROM LG_003_01_STLINE AS STLINE 
			LEFT JOIN LG_003_01_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = 3
			LEFT JOIN LG_003_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
			WHERE STLINE.IOCODE IN (1,2) AND STLINE.STOCKREF = {productReferenceId}";
				JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
				if (jsonDocument != null)
				{
					List<RawProductInputTransactionModel> result = (List<RawProductInputTransactionModel>)jsonDocument.Deserialize(typeof(List<RawProductInputTransactionModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
					if (result != null)
					{
						foreach (RawProductInputTransactionModel item in result)
						{

							yield return item;
						}
					}
				}
			}
		}
        public async IAsyncEnumerable<RawProductOutputTransactionModel> GetOutputRawProduct(int productReferenceId)
        {
			RawProductOutputTransactionModel viewModel = new RawProductOutputTransactionModel();
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			if (viewModel != null)
			{
				string query = $@"SELECT 
			STLINE.LOGICALREF AS [ReferenceId],
			STLINE.DATE_ AS [TransactionDate],
			ISNULL(STLINE.AMOUNT,0) AS [Quantity],
			STLINE.LINEEXP AS [Description],
			UNITSET.CODE AS [UnitsetCode],
			STFICHE.LOGICALREF AS [ProductTransactionReferenceId],
			STFICHE.FICHENO AS [TransactionCode],
			STFICHE.TRCODE AS [TransactionType],
			CAPIWHOUSE.LOGICALREF AS [WarehouseReferenceId],
			CAPIWHOUSE.NR AS [WarehouseNumber],
			CAPIWHOUSE.NAME AS [WarehouseName]
			FROM LG_003_01_STLINE AS STLINE 
			LEFT JOIN LG_003_01_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = 3
			LEFT JOIN LG_003_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
			WHERE STLINE.IOCODE IN (3,4) AND STLINE.STOCKREF = {productReferenceId}";
				JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
				if (jsonDocument != null)
				{
					List<RawProductOutputTransactionModel> result = (List<RawProductOutputTransactionModel>)jsonDocument.Deserialize(typeof(List<RawProductOutputTransactionModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
					if (result != null)
					{
						foreach (RawProductOutputTransactionModel item in result)
						{

							yield return item;
						}
					}
				}
			}
		}

        public async IAsyncEnumerable<RawProductWarehouseTotalModel> GetWarehouseTotalRawProduct(int productReferenceId)
        {

			RawProductWarehouseTotalModel viewModel = new RawProductWarehouseTotalModel();
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			if (viewModel != null)
			{
				string query = $@"SELECT 
			WHOUSE.LOGICALREF AS [ReferenceId],
			[WarehouseNumber] = WHOUSE.NR,
			[WarehouseName] = WHOUSE.NAME,
			[LastTransactionDate] = (SELECT TOP 1 LASTTRDATE FROM LV_003_01_STINVTOT WHERE STOCKREF = {productReferenceId} AND INVENNO = WHOUSE.NR ORDER BY LASTTRDATE	DESC),
			[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_003_01_STINVTOT WHERE STOCKREF = {productReferenceId} AND INVENNO = WHOUSE.NR),0)
			FROM
			L_CAPIWHOUSE AS WHOUSE
			WHERE WHOUSE.FIRMNR = 3";
				JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
				if (jsonDocument != null)
				{
					List<RawProductWarehouseTotalModel> result = (List<RawProductWarehouseTotalModel>)jsonDocument.Deserialize(typeof(List<RawProductWarehouseTotalModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
					if (result != null)
					{
						foreach (RawProductWarehouseTotalModel item in result)
						{

							yield return item;
						}
					}
				}
			}
		}
        private async IAsyncEnumerable<RawProductWaitingSalesOrderModel> GetRawProductBySalesOrderLine(int productReferenceId)
		{
			RawProductWaitingSalesOrderModel viewModel = new RawProductWaitingSalesOrderModel();
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
                                WHERE (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.CLOSED = 0 AND ORFLINE.TRCODE =  1 AND ORFLINE.STOCKREF = {productReferenceId}";
				JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
				if (jsonDocument != null)
				{
					List<RawProductWaitingSalesOrderModel> result = (List<RawProductWaitingSalesOrderModel>)jsonDocument.Deserialize(typeof(List<RawProductWaitingSalesOrderModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
					if (result != null)
					{
						foreach (RawProductWaitingSalesOrderModel item in result)
						{

							yield return item;
						}
					}
				}
			}
		}
        private async IAsyncEnumerable<RawProductWaitingPurchaseOrderModel> GetRawProductByPurchaseOrderLine(int productReferenceId)
        {
			RawProductWaitingPurchaseOrderModel viewModel = new RawProductWaitingPurchaseOrderModel();
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
                                WHERE (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.CLOSED = 0 AND ORFLINE.TRCODE =  2 AND ORFLINE.STOCKREF = {productReferenceId}";
				JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
				if (jsonDocument != null)
				{
					List<RawProductWaitingPurchaseOrderModel> result = (List<RawProductWaitingPurchaseOrderModel>)jsonDocument.Deserialize(typeof(List<RawProductWaitingPurchaseOrderModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
					if (result != null)
					{
						foreach (RawProductWaitingPurchaseOrderModel item in result)
						{

							yield return item;
						}
					}
				}
			}
		}

        private async IAsyncEnumerable<RawProductMeasureModel> GetProductMesaure(int productReferenceId)
        {
            RawProductMeasureModel viewModel = new RawProductMeasureModel();
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            if (viewModel != null)
            {
                string query = $@"SELECT 
            [Barcode] = (SELECT BARCODE FROM LG_003_UNITBARCODE	
            			WHERE ITEMREF = ITMUNITA.ITEMREF AND ITMUNITAREF = ITMUNITA.LOGICALREF AND UNITLINEREF = ITMUNITA.UNITLINEREF),
            ITMUNITA.WIDTH AS [Width],
            ITMUNITA.HEIGHT AS [Height],
            ITMUNITA.VOLUME_ AS [Volume],
            ITMUNITA.WEIGHT AS [Weight],
            UNITSETL.CODE AS [SubunitsetCode]
            FROM LG_001_ITMUNITA AS ITMUNITA
            LEFT JOIN LG_003_UNITSETL AS UNITSETL ON ITMUNITA.UNITLINEREF = UNITSETL.LOGICALREF
            WHERE ITEMREF = {productReferenceId}";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<RawProductMeasureModel> result = (List<RawProductMeasureModel>)jsonDocument.Deserialize(typeof(List<RawProductMeasureModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (RawProductMeasureModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }
        }

        private async IAsyncEnumerable<RawProductWarehouseParameterModel> GetWarehouseParameterEndProduct(int productReferenceId)
        {
            RawProductWarehouseParameterModel viewModel = new RawProductWarehouseParameterModel();
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            if (viewModel != null)
            {
                string query = $@"SELECT 
        INVDEF.LOGICALREF AS [ReferenceId],
        WAREHOUSE.NR AS [InventoryNo],
        WAREHOUSE.NAME AS [WarehouseName],
        INVDEF.MINLEVEL AS [MinimumLevel],
        INVDEF.MAXLEVEL AS [MaximumLevel],
        INVDEF.SAFELEVEL AS [SafeLevel],
        [StockQuantity] = ISNULL((SELECT SUM(DISTINCT ONHAND) FROM LV_003_01_STINVTOT AS STINVTOT WHERE STINVTOT.STOCKREF = {productReferenceId} AND STINVTOT.INVENNO = WAREHOUSE.NR),0)
        FROM LG_003_INVDEF AS INVDEF
        LEFT JOIN L_CAPIWHOUSE AS WAREHOUSE ON INVDEF.INVENNO = WAREHOUSE.NR AND WAREHOUSE.FIRMNR = 3
        WHERE ITEMREF = {productReferenceId}";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<RawProductWarehouseParameterModel> result = (List<RawProductWarehouseParameterModel>)jsonDocument.Deserialize(typeof(List<RawProductWarehouseParameterModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (RawProductWarehouseParameterModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }
        }
    }
}
