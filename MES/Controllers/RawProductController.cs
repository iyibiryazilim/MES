﻿using AutoMapper;
using LBS.Shared.Entity.BaseModels;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.Models.RawProductModels;
using MES.Models.SemiProductModels;
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
        public async Task<IActionResult> Detail(int referenceId)
        {
            RawProductDetailModel viewModel = new RawProductDetailModel();
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var product = await _service.GetObject(httpClient, referenceId);

            if (httpClient == null)
                BadRequest();

            if (product == null)
            {
                return NotFound();
            }
            viewModel.RawProductModel = _mapper.Map<RawProductModel>(product);
            viewModel.RawProductModel.RevolutionSpeed = 0;

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

        public async ValueTask<IActionResult> GetWarehouseJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetWarehouseRawProduct(productReferenceId) });
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

        public async IAsyncEnumerable<ProductTransactionLine> GetInputRawProduct(int productReferenceId)
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _transactionLineService.GetInputProductTransactionLineByProductRef(httpClient, productReferenceId);
            //Console.WriteLine(productReferenceId.ToString());
            await foreach (var item in result)
            {
                //Console.WriteLine(item.IOType.ToString());
                yield return item;
            }
        }
        public async IAsyncEnumerable<ProductTransactionLine> GetOutputRawProduct(int productReferenceId)
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _transactionLineService.GetOutputProductTransactionLineByProductRef(httpClient, productReferenceId);
            //Console.WriteLine(productReferenceId.ToString());
            await foreach (var item in result)
            {
                //Console.WriteLine(item.IOType.ToString());
                yield return item;
            }
        }

        public async IAsyncEnumerable<WarehouseTotal> GetWarehouseRawProduct(int productReferenceId)
        {

            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _warehouseTotalService.GetObjectsAsyncByProduct(httpClient, productReferenceId);
            //Console.WriteLine(productReferenceId.ToString());
            await foreach (var item in result)
            {
                //Console.WriteLine(item.IOType.ToString());
                yield return item;
            }
        }
        private async IAsyncEnumerable<SalesOrderLine> GetRawProductBySalesOrderLine(int productReferenceId)
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _salesOrderLineService.GetObjectsByProductRef(httpClient, productReferenceId);
            await foreach (var item in result)
            {
                yield return item;
            }
        }
        private async IAsyncEnumerable<PurchaseOrderLine> GetRawProductByPurchaseOrderLine(int productReferenceId)
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _purchaseOrderLineService.GetObjectsByProductRef(httpClient, productReferenceId);
            await foreach (var item in result)
            {
                yield return item;
            }
        }
    }
}
