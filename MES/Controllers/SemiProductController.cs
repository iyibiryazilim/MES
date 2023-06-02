using AutoMapper;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.ProductModels.EndProductModels;
using MES.Models.ProductModels.RawProductModels;
using MES.Models.ProductModels.SemiProductModels;
using MES.ViewModels.ProductViewModels.EndProductViewModels;
using MES.ViewModels.ProductViewModels.SemiProductviewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers
{
    public class SemiProductController : Controller
    {
        readonly ILogger<SemiProductController> _logger;
        readonly ISemiProductService _service;
        readonly IHttpClientService _httpClientService;
        readonly IMapper _mapper;
        readonly IProductTransactionLineService _transactionLineService;
        readonly IWarehouseTotalService _warehouseTotalService;
        readonly IProductWarehouseParameterService _warehouseParameterService;
        readonly IProductMeasureService _productMeasureService;
        readonly ICustomQueryService _customQueryService;
        readonly ISalesOrderLineService _salesOrderLineService;
        readonly IPurchaseOrderLineService _purchaseOrderLineService;

        public SemiProductController(ILogger<SemiProductController> logger,
            IHttpClientService httpClientService,
            ISemiProductService service,
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
            ViewData["Title"] = "Yarı Mamuller";
            return View();
        }

        public async Task<IActionResult> Detail(int productReferenceId)
        {
			ViewData["Header"] = "Yarı Mamul Detay";
			SemiProductDetailViewModel viewModel = new SemiProductDetailViewModel();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			if (httpClient == null)
				return BadRequest();
			else
			{
				var product = await _service.GetObject(httpClient, productReferenceId);

				if (product == null)
					return NotFound();

                string query = $@" 
                                SELECT 
 [DailyInputStock] = ISNULL((SELECT SUM(AMOUNT) FROM LG_003_01_STLINE 
 			WHERE IOCODE IN(1,2) AND STOCKREF = {productReferenceId} AND
 			DAY(DATE_) = DAY(GETDATE()) AND
 			MONTH(DATE_) = MONTH(GETDATE()) AND 
 			YEAR(DATE_) = YEAR(GETDATE())),0),
 
 [WeeklyInputStock]= ISNULL((SELECT SUM(AMOUNT) FROM LG_003_01_STLINE 
 			WHERE IOCODE IN(1,2) AND STOCKREF = {productReferenceId} AND
 			DATEPART(WEEK, DATE_) = DATEPART(WEEK, GETDATE())  AND 
 			YEAR(DATE_) = YEAR(GETDATE())),0),
 
 [MonthlyInputStock] = ISNULL((SELECT SUM(AMOUNT) FROM LG_003_01_STLINE 
 			WHERE IOCODE IN(1,2) AND STOCKREF = {productReferenceId} AND
 			MONTH(DATE_) = MONTH(GETDATE()) AND 
 			YEAR(DATE_) = YEAR(GETDATE())),0),
 
 [YearlyInputStock] = ISNULL((SELECT SUM(AMOUNT) FROM LG_003_01_STLINE 
    WHERE IOCODE IN(1,2) AND STOCKREF = {productReferenceId} AND
    YEAR(DATE_) = YEAR(GETDATE())),0),

 [DailyOutputStock] = ISNULL((SELECT SUM(AMOUNT) FROM LG_003_01_STLINE 
 			WHERE IOCODE IN(3,4) AND STOCKREF = {productReferenceId} AND
 			DAY(DATE_) = DAY(GETDATE()) AND
 			MONTH(DATE_) = MONTH(GETDATE()) AND 
 			YEAR(DATE_) = YEAR(GETDATE())),0),
 
 [WeeklyOutputStock]= ISNULL((SELECT SUM(AMOUNT) FROM LG_003_01_STLINE 
 			WHERE IOCODE IN(3,4) AND STOCKREF = {productReferenceId} AND
 			DATEPART(WEEK, DATE_) = DATEPART(WEEK, GETDATE()) AND 
 			YEAR(DATE_) = YEAR(GETDATE())),0),
 
 [MonthlyOutputStock] = ISNULL((SELECT SUM(AMOUNT) FROM LG_003_01_STLINE 
 			WHERE IOCODE IN(3,4) AND STOCKREF = {productReferenceId} AND
 			MONTH(DATE_) = MONTH(GETDATE()) AND 
 			YEAR(DATE_) = YEAR(GETDATE())),0),
 
 [YearlyOutputStock] = ISNULL((SELECT SUM(AMOUNT) FROM LG_003_01_STLINE 
    WHERE IOCODE IN(3,4) AND STOCKREF = {productReferenceId} AND
    YEAR(DATE_) = YEAR(GETDATE())),0)

                            ";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<SemiProductDetailViewModel> result = (List<SemiProductDetailViewModel>)jsonDocument.Deserialize(typeof(List<SemiProductDetailViewModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (SemiProductDetailViewModel item in result)
                        {

                            viewModel = item;
                        }
                    }
                }

                #region Total Input
                var totalInputResult = await _customQueryService.GetObjects(httpClient, $@"SELECT ISNULL(SUM(DISTINCT AMOUNT),0) AS [TotalInput] FROM LG_003_01_STLINE WHERE IOCODE IN(1,2) AND STOCKREF = {productReferenceId}");

                var inputResult = totalInputResult.RootElement.EnumerateArray();
                foreach (JsonElement element in inputResult)
                {
                    JsonElement totalInput = element.GetProperty("totalInput");
                    viewModel.TotalInput = Convert.ToDouble(totalInput.GetRawText().Replace('.', ','));
                }
                #endregion

                #region Total Output
                var totalOutputResult = await _customQueryService.GetObjects(httpClient, $@"SELECT ISNULL(SUM(DISTINCT AMOUNT),0) AS [TotalOutput] FROM LG_003_01_STLINE WHERE IOCODE IN(3,4) AND STOCKREF = {productReferenceId}");

                var outputResult = totalOutputResult.RootElement.EnumerateArray();
                foreach (JsonElement element in outputResult)
                {
                    JsonElement totalOutput = element.GetProperty("totalOutput");
                    viewModel.TotalOutput = Convert.ToDouble(totalOutput.GetRawText().Replace('.', ','));
                }
                #endregion



                if (viewModel.TotalInput == 0 || viewModel.TotalOutput == 0)
                {
                    viewModel.InputOutputRate = 0;
                }
                else
                {
                    viewModel.InputOutputRate = Math.Round((viewModel.TotalInput / viewModel.TotalOutput) * 100, 2);
                }


                viewModel.SemiProductModel = _mapper.Map<SemiProductModel>(product);
				viewModel.SemiProductModel.OutputQuantity = 0;
				viewModel.SemiProductModel.StockQuantity = 0;
				viewModel.SemiProductModel.FirstQuantity = 0;
				viewModel.SemiProductModel.InputQuantity = 0;
				viewModel.SemiProductModel.RevolutionSpeed = RevolutionSpeed(productReferenceId);

                await foreach (SemiProductMeasureModel model in GetProductMesaure(productReferenceId))
                    viewModel.SemiProductMeasureModel.Add(model);



            }

			return View(viewModel);
		}
        public async ValueTask<IActionResult> GetSemiProductJsonResult()
        {
            return Json(new { data = GetSemiProduct() });
        }
        public async ValueTask<IActionResult> GetInputJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetInputSemiProduct(productReferenceId) });
        }
        public async ValueTask<IActionResult> GetOutputJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetOutputSemiProduct(productReferenceId) });
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
            return Json(new { data = GetWarehouseSemiProduct(productReferenceId) });
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

        public async ValueTask<IActionResult> GetMonthlyInputJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetMonthlyInputModels(productReferenceId) });
        }

        public async ValueTask<IActionResult> GetMonthlyOutputJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetMonthlyOutputModels(productReferenceId) });
        }
        public async IAsyncEnumerable<SemiProductListModel> GetSemiProduct()
        {
            SemiProductListModel viewModel = new SemiProductListModel();
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
  ITEM.CARDTYPE = 11


";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<SemiProductListModel> result = (List<SemiProductListModel>)jsonDocument.Deserialize(typeof(List<SemiProductListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (SemiProductListModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }


        }

        public async IAsyncEnumerable<SemiProductInputTransactionModel> GetInputSemiProduct(int productReferenceId)
        {
			SemiProductInputTransactionModel viewModel = new SemiProductInputTransactionModel();
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
					List<SemiProductInputTransactionModel> result = (List<SemiProductInputTransactionModel>)jsonDocument.Deserialize(typeof(List<SemiProductInputTransactionModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
					if (result != null)
					{
						foreach (SemiProductInputTransactionModel item in result)
						{

							yield return item;
						}
					}
				}
			}
		}
        public async IAsyncEnumerable<SemiProductOutputTransactionModel> GetOutputSemiProduct(int productReferenceId)
        {
			SemiProductOutputTransactionModel viewModel = new SemiProductOutputTransactionModel();
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
					List<SemiProductOutputTransactionModel> result = (List<SemiProductOutputTransactionModel>)jsonDocument.Deserialize(typeof(List<SemiProductOutputTransactionModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
					if (result != null)
					{
						foreach (SemiProductOutputTransactionModel item in result)
						{

							yield return item;
						}
					}
				}
			}
		}

        public async IAsyncEnumerable<SemiProductWarehouseTotalModel> GetWarehouseSemiProduct(int productReferenceId)
        {
			SemiProductWarehouseTotalModel viewModel = new SemiProductWarehouseTotalModel();
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
					List<SemiProductWarehouseTotalModel> result = (List<SemiProductWarehouseTotalModel>)jsonDocument.Deserialize(typeof(List<SemiProductWarehouseTotalModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
					if (result != null)
					{
						foreach (SemiProductWarehouseTotalModel item in result)
						{

							yield return item;
						}
					}
				}
			}
		}
        private async IAsyncEnumerable<SemiProductWaitingSalesOrderModel> GetRawProductBySalesOrderLine(int productReferenceId)
        {
			SemiProductWaitingSalesOrderModel viewModel = new SemiProductWaitingSalesOrderModel();
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
					List<SemiProductWaitingSalesOrderModel> result = (List<SemiProductWaitingSalesOrderModel>)jsonDocument.Deserialize(typeof(List<SemiProductWaitingSalesOrderModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
					if (result != null)
					{
						foreach (SemiProductWaitingSalesOrderModel item in result)
						{

							yield return item;
						}
					}
				}
			}
		}
        private async IAsyncEnumerable<SemiProductWaitingPurchaseOrderModel> GetRawProductByPurchaseOrderLine(int productReferenceId)
        {
			SemiProductWaitingPurchaseOrderModel viewModel = new SemiProductWaitingPurchaseOrderModel();
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
					List<SemiProductWaitingPurchaseOrderModel> result = (List<SemiProductWaitingPurchaseOrderModel>)jsonDocument.Deserialize(typeof(List<SemiProductWaitingPurchaseOrderModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
					if (result != null)
					{
						foreach (SemiProductWaitingPurchaseOrderModel item in result)
						{

							yield return item;
						}
					}
				}
			}
		}

        private async IAsyncEnumerable<SemiProductMeasureModel> GetProductMesaure(int productReferenceId)
        {
            SemiProductMeasureModel viewModel = new SemiProductMeasureModel();
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
                    List<SemiProductMeasureModel> result = (List<SemiProductMeasureModel>)jsonDocument.Deserialize(typeof(List<SemiProductMeasureModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (SemiProductMeasureModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }
        }

        private async IAsyncEnumerable<SemiProductWarehouseParameterModel> GetWarehouseParameterEndProduct(int productReferenceId)
        {
            SemiProductWarehouseParameterModel viewModel = new SemiProductWarehouseParameterModel();
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
                    List<SemiProductWarehouseParameterModel> result = (List<SemiProductWarehouseParameterModel>)jsonDocument.Deserialize(typeof(List<SemiProductWarehouseParameterModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (SemiProductWarehouseParameterModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }
        }

        private async IAsyncEnumerable<SemiProductMonthlyInputModel> GetMonthlyInputModels(int productReferenceId)
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            SemiProductMonthlyInputModel model = new SemiProductMonthlyInputModel();

            #region MonthlyInputChart
            var monthlyInputChart = await _customQueryService.GetObjects(httpClient, $@"WITH AllMonths AS (
                            SELECT 1 AS MonthNumber UNION ALL
                            SELECT 2 UNION ALL
                            SELECT 3 UNION ALL
                            SELECT 4 UNION ALL
                            SELECT 5 UNION ALL
                            SELECT 6 UNION ALL
                            SELECT 7 UNION ALL
                            SELECT 8 UNION ALL
                            SELECT 9 UNION ALL
                            SELECT 10 UNION ALL
                            SELECT 11 UNION ALL
                            SELECT 12
                        ),
                        AggregatedData AS (
                            SELECT MONTH(STLINE.DATE_) AS [Month], ISNULL(SUM(STLINE.AMOUNT), 0) AS [TotalAmount]
                            FROM LG_003_01_STLINE AS STLINE
                            WHERE MONTH(STLINE.DATE_) <= MONTH(GETDATE()) AND YEAR(STLINE.DATE_) = YEAR(GETDATE()) AND STOCKREF = {productReferenceId} AND IOCODE IN(1,2)
                            GROUP BY MONTH(STLINE.DATE_)
                        )
                        SELECT TOP 5
                            AllMonths.MonthNumber,
                            COALESCE(AggregatedData.[TotalAmount], 0) AS [TotalAmount]
                        FROM AllMonths
                        LEFT JOIN AggregatedData ON AllMonths.MonthNumber = AggregatedData.[Month]
                        WHERE AllMonths.MonthNumber <= MONTH(GETDATE())
                        ORDER BY AllMonths.MonthNumber DESC
                        
                        ");

            var inputResultMonthly = monthlyInputChart.RootElement.EnumerateArray();
            foreach (JsonElement element in inputResultMonthly)
            {

                JsonElement inputMonth = element.GetProperty("monthNumber");
                var month = Convert.ToInt16(inputMonth.GetRawText());

                JsonElement inputTotalAmount = element.GetProperty("totalAmount");
                var totalAmount = Convert.ToDouble(inputTotalAmount.GetRawText().Replace('.', ','));
                model.MonthlyInputValues.Add(month, totalAmount);

                yield return model;
            }
            #endregion
        }

        private async IAsyncEnumerable<SemiProductMonthlyOutputModel> GetMonthlyOutputModels(int productReferenceId)
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            SemiProductMonthlyOutputModel model = new SemiProductMonthlyOutputModel();

            #region MonthlyOutputChart
            var monthlyOutputChart = await _customQueryService.GetObjects(httpClient, $@"WITH AllMonths AS (
                            SELECT 1 AS MonthNumber UNION ALL
                            SELECT 2 UNION ALL
                            SELECT 3 UNION ALL
                            SELECT 4 UNION ALL
                            SELECT 5 UNION ALL
                            SELECT 6 UNION ALL
                            SELECT 7 UNION ALL
                            SELECT 8 UNION ALL
                            SELECT 9 UNION ALL
                            SELECT 10 UNION ALL
                            SELECT 11 UNION ALL
                            SELECT 12
                        ),
                        AggregatedData AS (
                            SELECT MONTH(STLINE.DATE_) AS [Month], ISNULL(SUM(STLINE.AMOUNT), 0) AS [TotalAmount]
                            FROM LG_003_01_STLINE AS STLINE
                            WHERE MONTH(STLINE.DATE_) <= MONTH(GETDATE()) AND YEAR(STLINE.DATE_) = YEAR(GETDATE()) AND STOCKREF = {productReferenceId} AND IOCODE IN(3,4)
                            GROUP BY MONTH(STLINE.DATE_)
                        )
                        SELECT TOP 5
                            AllMonths.MonthNumber,
                            COALESCE(AggregatedData.[TotalAmount], 0) AS [TotalAmount]
                        FROM AllMonths
                        LEFT JOIN AggregatedData ON AllMonths.MonthNumber = AggregatedData.[Month]
                        WHERE AllMonths.MonthNumber <= MONTH(GETDATE())
                        ORDER BY AllMonths.MonthNumber DESC
                        
                        ");

            var outputResultMonthly = monthlyOutputChart.RootElement.EnumerateArray();
            foreach (JsonElement element in outputResultMonthly)
            {

                JsonElement outputMonth = element.GetProperty("monthNumber");
                var month = Convert.ToInt16(outputMonth.GetRawText());

                JsonElement outputTotalAmount = element.GetProperty("totalAmount");
                var totalAmount = Convert.ToDouble(outputTotalAmount.GetRawText().Replace('.', ','));
                model.MonthlyOutputValues.Add(month, totalAmount);

                yield return model;
            }
            #endregion
        }
        private async ValueTask<double> RevolutionSpeed(int productReferenceId)
        {
            //Stok Devir Hızı = (Satılan Malların Maliyeti / (Başlangıç Stok Değeri + Dönem Sonu Stok Değeri) / 2)

            double _revolutionSpeed = 5;

            return _revolutionSpeed;
        }

    }
}
