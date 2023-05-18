using AutoMapper;
using LBS.Shared.Entity.BaseModels;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.RawProductViewModel;
using Microsoft.AspNetCore.Mvc;

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
        public RawProductController(ILogger<RawProductController> logger,
            IHttpClientService httpClientService,
            IRawProductService service, IMapper mapper, IProductTransactionLineService transactionLineService, IWarehouseTotalService warehouseTotalService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _mapper = mapper;
            _transactionLineService = transactionLineService;
            _warehouseTotalService = warehouseTotalService;
        }


        public IActionResult Index()
        {
            ViewData["Title"] = "Hammaddeler";
            return View();
        }
        public async Task<IActionResult> Detail(int referenceId)
        {
            RawProductDetailViewModel viewModel = new RawProductDetailViewModel();
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var product = await _service.GetObject(httpClient, referenceId);

            if (httpClient == null)
                BadRequest();

            if (product == null)
                return NotFound();

            viewModel.RawProductModel = _mapper.Map<RawProductModel>(product);
            viewModel.RawProductModel.SellQuentity = 0;
            viewModel.RawProductModel.FirstQuentity = 0;
            viewModel.RawProductModel.StockQuentity = 0;
            viewModel.RawProductModel.PurchaseQuentity = 0;
            viewModel.RawProductModel.RevolutionSpeed = 0;
            viewModel.RawProductModel.DailyStock = 0;
            viewModel.RawProductModel.DailyStockChange = 0;
            viewModel.RawProductModel.WeeklyStock = 0;
            viewModel.RawProductModel.WeeklyStockChange = 0;
            viewModel.RawProductModel.MonthlyStock = 0;
            viewModel.RawProductModel.MonthlyStockChange = 0;
            viewModel.RawProductModel.YearlyStock = 0;
            viewModel.RawProductModel.YearlyStockChange = 0;



            ViewData["Title"] = viewModel.RawProductModel.Name;
            return View(viewModel);
        }


        public async ValueTask<IActionResult> GetJsonResult()
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

        public async ValueTask<IActionResult> GetWarehouseJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetWarehouseRawProduct(productReferenceId) });
        }


        public async IAsyncEnumerable<RawProductListViewModel> GetRawProduct()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _service.GetObjects(httpClient);

            await foreach (var item in result)
            {
                yield return new RawProductListViewModel
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
                    stockQuentity = 55
                };
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
            var result = _transactionLineService.GetInputProductTransactionLineByProductRef(httpClient, productReferenceId);
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
    }
}
