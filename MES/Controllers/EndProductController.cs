using AutoMapper;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.ProductModels.EndProductModels;
using MES.ViewModels.ProductViewModels.EndProductViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using System.Text.Json;

namespace MES.Controllers;

public class EndProductController : Controller
{
    readonly ILogger<EndProductController> _logger;
    readonly IEndProductService _service;
    readonly IHttpClientService _httpClientService;
    readonly IMapper _mapper;
    readonly IProductTransactionLineService _transactionLineService;
    readonly IWarehouseTotalService _warehouseTotalService;
    readonly IProductWarehouseParameterService _warehouseParameterService;
    readonly IProductMeasureService _productMeasureService;
    readonly ICustomQueryService _customQueryService;
    readonly ISalesOrderLineService _salesOrderLineService;
    readonly IPurchaseOrderLineService _purchaseOrderLineService;

    public EndProductController(ILogger<EndProductController> logger,
        IHttpClientService httpClientService,
        IEndProductService service,
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
        ViewData["Title"] = "Mamuller";
        return View();
    }

    public async Task<IActionResult> Detail(int productReferenceId)
    {
        
        EndProductDetailViewModel viewModel = new EndProductDetailViewModel();
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
                List<EndProductDetailViewModel> result = (List<EndProductDetailViewModel>)jsonDocument.Deserialize(typeof(List<EndProductDetailViewModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (EndProductDetailViewModel item in result)
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
                viewModel.TotalInput = Convert.ToDouble(totalInput.GetRawText());
            }
            #endregion

            #region Total Output
            var totalOutputResult = await _customQueryService.GetObjects(httpClient, $@"SELECT ISNULL(SUM(DISTINCT AMOUNT),0) AS [TotalOutput] FROM LG_003_01_STLINE WHERE IOCODE IN(3,4) AND STOCKREF = {productReferenceId}");

            var outputResult = totalOutputResult.RootElement.EnumerateArray();
            foreach (JsonElement element in outputResult)
            {
                JsonElement totalInput = element.GetProperty("totalOutput");
                viewModel.TotalOutput = Convert.ToDouble(totalInput.GetRawText());
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



            viewModel.EndProductModel = _mapper.Map<EndProductModel>(product);
            viewModel.EndProductModel.OutputQuantity = 0;
            viewModel.EndProductModel.StockQuantity = 0;
            viewModel.EndProductModel.FirstQuantity = 0;
            viewModel.EndProductModel.InputQuantity = 0;
            viewModel.EndProductModel.RevolutionSpeed = RevolutionSpeed(productReferenceId);


            await foreach (EndProductMeasureModel model in GetProductMesaure(productReferenceId))
                viewModel.EndProductMeasureModel.Add(model);



        }
		ViewData["Header"] = "Mamul Detay";
		return View(viewModel);
    }

    public async ValueTask<IActionResult> GetEndProductJsonResult()
    {
        return Json(new { data = GetEndProducts() });
    }
    public async ValueTask<IActionResult> GetInputJsonResult(int productReferenceId)
    {
        //Console.WriteLine(productReferenceId.ToString());
        return Json(new { data = GetInputEndProduct(productReferenceId) });
    }
    public async ValueTask<IActionResult> GetSalesOrderLineJsonResult(int productReferenceId)
    {
        //Console.WriteLine(productReferenceId.ToString());
        return Json(new { data = GetEndProductBySalesOrderLine(productReferenceId) });
    }
    public async ValueTask<IActionResult> GetPurchaseOrderLineJsonResult(int productReferenceId)
    {
        //Console.WriteLine(productReferenceId.ToString());
        return Json(new { data = GetEndProductByPurchaseOrderLine(productReferenceId) });
    }
    public async ValueTask<IActionResult> GetOutputJsonResult(int productReferenceId)
    {
        //Console.WriteLine(productReferenceId.ToString());
        return Json(new { data = GetOutputEndProduct(productReferenceId) });
    }
    public async ValueTask<IActionResult> GetWarehouseTotalJsonResult(int productReferenceId)
    {
        //Console.WriteLine(productReferenceId.ToString());
        return Json(new { data = GetWarehouseEndProduct(productReferenceId) });
    }

    public async ValueTask<IActionResult> GetWarehouseParameterJsonResult(int productReferenceId)
    {
        //Console.WriteLine(productReferenceId.ToString());
        return Json(new { data = GetWarehouseParameterEndProduct(productReferenceId) });
    }

    public async ValueTask<IActionResult> GetProductMesaureJsonResult(int productReferenceId)
    {
        //Console.WriteLine(productReferenceId.ToString());
        return Json(new { data = GetProductMesaure(productReferenceId) });
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



    private async IAsyncEnumerable<EndProductListModel> GetEndProducts()
    {
        EndProductListModel viewModel = new EndProductListModel();
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
  ITEM.CARDTYPE = 12


";
            JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
            if (jsonDocument != null)
            {
                List<EndProductListModel> result = (List<EndProductListModel>)jsonDocument.Deserialize(typeof(List<EndProductListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (EndProductListModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }


    }

    private async IAsyncEnumerable<EndProductInputTransactionModel> GetInputEndProduct(int productReferenceId)
    {
        EndProductInputTransactionModel viewModel = new EndProductInputTransactionModel();
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
                List<EndProductInputTransactionModel> result = (List<EndProductInputTransactionModel>)jsonDocument.Deserialize(typeof(List<EndProductInputTransactionModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (EndProductInputTransactionModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }
    }
    private async IAsyncEnumerable<EndProductOutputTransactionModel> GetOutputEndProduct(int productReferenceId)
    {
        EndProductOutputTransactionModel viewModel = new EndProductOutputTransactionModel();
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
                List<EndProductOutputTransactionModel> result = (List<EndProductOutputTransactionModel>)jsonDocument.Deserialize(typeof(List<EndProductOutputTransactionModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (EndProductOutputTransactionModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }
    }

    private async IAsyncEnumerable<EndProductWarehouseTotalModel> GetWarehouseEndProduct(int productReferenceId)
    {
        EndProductWarehouseTotalModel viewModel = new EndProductWarehouseTotalModel();
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
                List<EndProductWarehouseTotalModel> result = (List<EndProductWarehouseTotalModel>)jsonDocument.Deserialize(typeof(List<EndProductWarehouseTotalModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (EndProductWarehouseTotalModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }
    }

    private async IAsyncEnumerable<EndProductWaitingSalesOrderModel> GetEndProductBySalesOrderLine(int productReferenceId)
    {
        EndProductWaitingSalesOrderModel viewModel = new EndProductWaitingSalesOrderModel();
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
                List<EndProductWaitingSalesOrderModel> result = (List<EndProductWaitingSalesOrderModel>)jsonDocument.Deserialize(typeof(List<EndProductWaitingSalesOrderModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (EndProductWaitingSalesOrderModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }
    }
    private async IAsyncEnumerable<EndProductWaitingPurchaseOrderModel> GetEndProductByPurchaseOrderLine(int productReferenceId)
    {
        EndProductWaitingPurchaseOrderModel viewModel = new EndProductWaitingPurchaseOrderModel();
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
                List<EndProductWaitingPurchaseOrderModel> result = (List<EndProductWaitingPurchaseOrderModel>)jsonDocument.Deserialize(typeof(List<EndProductWaitingPurchaseOrderModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (EndProductWaitingPurchaseOrderModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }
    }

    private async IAsyncEnumerable<EndProductWarehouseParameterModel> GetWarehouseParameterEndProduct(int productReferenceId)
    {
        EndProductWarehouseParameterModel viewModel = new EndProductWarehouseParameterModel();
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
                List<EndProductWarehouseParameterModel> result = (List<EndProductWarehouseParameterModel>)jsonDocument.Deserialize(typeof(List<EndProductWarehouseParameterModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (EndProductWarehouseParameterModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }
    }

    private async IAsyncEnumerable<EndProductMeasureModel> GetProductMesaure(int productReferenceId)
    {
        EndProductMeasureModel viewModel = new EndProductMeasureModel();
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
                List<EndProductMeasureModel> result = (List<EndProductMeasureModel>)jsonDocument.Deserialize(typeof(List<EndProductMeasureModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (EndProductMeasureModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }
    }

    private async ValueTask<double> RevolutionSpeed(int productReferenceId)
    {
        //Stok Devir Hızı = (Satılan Malların Maliyeti / (Başlangıç Stok Değeri + Dönem Sonu Stok Değeri) / 2)

        double _revolutionSpeed = 5;

        return _revolutionSpeed;
    }

    private async IAsyncEnumerable<EndProductMonthlyInputModel> GetMonthlyInputModels(int productReferenceId)
    {
        var httpClient = _httpClientService.GetOrCreateHttpClient();

        EndProductMonthlyInputModel model = new EndProductMonthlyInputModel();

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

    private async IAsyncEnumerable<EndProductMonthlyOutputModel> GetMonthlyOutputModels(int productReferenceId)
    {
        var httpClient = _httpClientService.GetOrCreateHttpClient();

        EndProductMonthlyOutputModel model = new EndProductMonthlyOutputModel();

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

}

