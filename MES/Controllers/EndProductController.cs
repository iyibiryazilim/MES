using AutoMapper;
using LBS.Shared.Entity.BaseModels;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.Models.EndProductModels;
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
        EndProductDetailModel viewModel = new EndProductDetailModel();
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        var product = await _service.GetObject(httpClient, referenceId);

        if (httpClient == null)
            BadRequest();

        if (product == null)
        {
            return NotFound();
        }
        viewModel.EndProductModel = _mapper.Map<EndProductListModel>(product);
        viewModel.EndProductModel.RevolutionSpeed = 0;

        var warehouseParameters = _warehouseParameterService.GetObjects(httpClient, referenceId);
        if (warehouseParameters != null)
        {
            await foreach (ProductWarehouseParameter warehouseParameter in warehouseParameters)
                viewModel.WarehouseParameters.Add(_mapper.Map<ProductWarehouseParameterModel>(warehouseParameter));
        }

        var measures = _productMeasureService.GetObjects(httpClient, referenceId);
        if (measures != null)
        {
            await foreach (ProductMeasure measure in measures)
                viewModel.ProductMeasures.Add(_mapper.Map<ProductMeasureModel>(measure));
        }


        //ViewData["Title"] = viewModel.EndProductModel.Name;
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
    public async ValueTask<IActionResult> GetWarehouseJsonResult(int productReferenceId)
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
            string query = $@"SELECT ITEM.LOGICALREF,ITEM.CODE,ITEM.NAME,ITEM.PRODUCERCODE,ITEM.SPECODE,UNITSET.CODE AS [UNITSETCODE],
						[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_003_01_STINVTOT WHERE STOCKREF = ITEM.LOGICALREF AND INVENNO = -1),0),
						[InputQuantity] = ISNULL((SELECT SUM(AMOUNT) FROM LG_003_01_STLINE WHERE STOCKREF = ITEM.LOGICALREF AND IOCODE IN(1,2)),0),
						[OutputQuantity] = ISNULL((SELECT SUM(AMOUNT) FROM LG_003_01_STLINE WHERE STOCKREF = ITEM.LOGICALREF AND IOCODE IN(3,4)),0),
						[LastTransactionDate] =(SELECT TOP 1 DATE_ FROM LG_003_01_STLINE WHERE STOCKREF = ITEM.LOGICALREF ORDER BY DATE_ DESC)
						FROM LG_003_ITEMS AS ITEM LEFT JOIN LG_003_UNITSETF AS UNITSET ON ITEM.UNITSETREF = UNITSET.LOGICALREF WHERE ITEM.CARDTYPE = 12";
            JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
            if (jsonDocument != null)
            {
                var array = jsonDocument.RootElement.EnumerateArray();
                foreach (JsonElement element in array)
                {
                    #region Reference Id
                    JsonElement referenceId = element.GetProperty("logicalref");
                    viewModel.ReferenceId = Convert.ToInt32(referenceId.GetRawText().Replace('.', ','));
                    #endregion
                    #region Product Name
                    JsonElement name = element.GetProperty("name");
                    viewModel.Name = name.GetString();
                    #endregion
                    #region Producer Code
                    JsonElement producerCode = element.GetProperty("producercode");
                    viewModel.ProducerCode = producerCode.GetString();
                    #endregion

                    #region Special Code
                    JsonElement speCode = element.GetProperty("specode");
                    viewModel.SpeCode = speCode.GetString();
                    #endregion
                    #region Special Code
                    JsonElement unitsetcode = element.GetProperty("unitsetcode");
                    viewModel.Unitset = unitsetcode.GetString();
                    #endregion

                    #region Revolution Speed
                    viewModel.RevolutionSpeed = 50;
                    #endregion

                    #region Stock Quantity
                    JsonElement stockQuantity = element.GetProperty("stockQuantity");
                    viewModel.StockQuantity = Convert.ToDouble(stockQuantity.GetRawText().Replace('.', ','));
                    #endregion

                    #region InputQuantity
                    JsonElement inputQuantity = element.GetProperty("inputQuantity");
                    viewModel.InputQuantity = Convert.ToDouble(inputQuantity.GetRawText().Replace('.', ','));
                    #endregion

                    #region OutputQuantity
                    JsonElement outputQuantity = element.GetProperty("outputQuantity");
                    viewModel.OutputQuantity = Convert.ToDouble(outputQuantity.GetRawText().Replace('.', ','));
                    #endregion

                    #region LastTransactionDate
                    JsonElement lastTransactionDate = element.GetProperty("lastTransactionDate");

                    if (element.TryGetProperty("lastTransactionDate", out lastTransactionDate) && lastTransactionDate.ValueKind != JsonValueKind.Null)
                    {
                        viewModel.LastTransactionDate = JsonSerializer.Deserialize<DateTime>(lastTransactionDate.GetRawText());

                    }
                    #endregion

                    yield return viewModel;
                }
            }
        }



    }

    public async IAsyncEnumerable<ProductTransactionLine> GetInputEndProduct(int productReferenceId)
    {
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        var result = _transactionLineService.GetInputProductTransactionLineByProductRef(httpClient, productReferenceId);
        //Console.WriteLine(productReferenceId.ToString());
        await foreach (var item in result)
        {
            yield return item;
        }
    }
    public async IAsyncEnumerable<ProductTransactionLine> GetOutputEndProduct(int productReferenceId)
    {
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        var result = _transactionLineService.GetOutputProductTransactionLineByProductRef(httpClient, productReferenceId);
        //Console.WriteLine(productReferenceId.ToString());
        await foreach (var item in result)
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<WarehouseTotal> GetWarehouseEndProduct(int productReferenceId)
    {
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        var result = _warehouseTotalService.GetObjectsAsyncByProduct(httpClient, productReferenceId);

        await foreach (var item in result)
        {
            //Console.WriteLine(item.Product.Name.ToString());
            yield return item;
        }
    }

    private async IAsyncEnumerable<SalesOrderLine> GetEndProductBySalesOrderLine(int productReferenceId)
    {
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        var result = _salesOrderLineService.GetObjectsByProductRef(httpClient, productReferenceId);
        await foreach (var item in result)
        {
            yield return item;
        }
    }
    private async IAsyncEnumerable<PurchaseOrderLine> GetEndProductByPurchaseOrderLine(int productReferenceId)
    {
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        var result = _purchaseOrderLineService.GetObjectsByProductRef(httpClient, productReferenceId);
        await foreach (var item in result)
        {
            yield return item;
        }
    }
}

