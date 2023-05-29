using AutoMapper;
using LBS.Shared.Entity.BaseModels;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.Models.EndProductModels;
using MES.Models.RawProductModels;
using MES.Models.SemiProductModels;
using MES.ViewModels.ProductViewModels.EndProductViewModels;
using MES.ViewModels.ProductViewModels.RawProductViewModels;
using Microsoft.AspNetCore.Mvc;
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

    public async Task<IActionResult> Detail(int referenceId)
    {
        ViewData["Header"] = "Mamul Detay";
        EndProductDetailViewModel viewModel = new EndProductDetailViewModel();
        var httpClient = _httpClientService.GetOrCreateHttpClient();
        if (httpClient == null)
            return BadRequest();
        else
        {
            var product = await _service.GetObject(httpClient, referenceId);

            if (product == null)
                return NotFound();


            viewModel.EndProductModel = _mapper.Map<EndProductModel>(product);
            viewModel.EndProductModel.OutputQuantity = 0;
            viewModel.EndProductModel.StockQuantity = 0;
            viewModel.EndProductModel.FirstQuantity = 0;
            viewModel.EndProductModel.InputQuantity = 0;
            viewModel.EndProductModel.RevolutionSpeed = 0;



        }

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

    public async IAsyncEnumerable<EndProductListModel> GetEndProducts()
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

    public async IAsyncEnumerable<EndProductInputTransactionModel> GetInputEndProduct(int productReferenceId)
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
    public async IAsyncEnumerable<EndProductOutputTransactionModel> GetOutputEndProduct(int productReferenceId)
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

    public async IAsyncEnumerable<EndProductWarehouseTotalModel> GetWarehouseEndProduct(int productReferenceId)
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
}

