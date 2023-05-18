using AutoMapper;
using LBS.Shared.Entity.BaseModels;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;

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

    public EndProductController(ILogger<EndProductController> logger,
        IHttpClientService httpClientService,
        IEndProductService service, IMapper mapper, IProductTransactionLineService transactionLineService, IWarehouseTotalService warehouseTotalService, IProductMeasureService productMeasureService, IProductWarehouseParameterService warehouseParameterService)
    {
        _logger = logger;
        _httpClientService = httpClientService;
        _service = service;
        _mapper = mapper;
        _transactionLineService = transactionLineService;
        _warehouseTotalService = warehouseTotalService;
        _productMeasureService = productMeasureService;
        _warehouseParameterService = warehouseParameterService;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Mamuller";
        return View();
    }

    public async Task<IActionResult> Detail(int referenceId)
    {
        EndProductDetailViewModel viewModel = new EndProductDetailViewModel();
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        var product = await _service.GetObject(httpClient, referenceId);

        if (httpClient == null)
            BadRequest();

        if (product == null)
            return NotFound();

        viewModel.EndProductModel = _mapper.Map<EndProductModel>(product);
        viewModel.EndProductModel.SellQuentity = 0;
        viewModel.EndProductModel.FirstQuentity = 0;
        viewModel.EndProductModel.StockQuentity = 0;
        viewModel.EndProductModel.PurchaseQuentity = 0;
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


        ViewData["Title"] = viewModel.EndProductModel.Name;
        return View(viewModel);
    }

    public async ValueTask<IActionResult> GetJsonResult()
    {
        return Json(new { data = GetEndProducts() });
    }
    public async ValueTask<IActionResult> GetInputJsonResult(int productReferenceId)
    {
        //Console.WriteLine(productReferenceId.ToString());
        return Json(new { data = GetInputEndProduct(productReferenceId) });
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

    public async IAsyncEnumerable<EndProductListViewModel> GetEndProducts()
    {
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        var result = _service.GetObjects(httpClient);

        await foreach (var item in result)
        {
            yield return new EndProductListViewModel
            {
                Brand = item.Brand,
                CardType = item.CardType,
                Code = item.Code,
                LockTrackingType = item.LockTrackingType,
                Name = item.Name,
                ProducerCode = item.ProducerCode,
                ReferenceId = item.ReferenceId,
                SpeCode = item.SpeCode,
                TrackingType = item.TrackingType,
                Unitset = item.Unitset,
                Vat = item.Vat,
                stockQuentity = 5
            };
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
}

