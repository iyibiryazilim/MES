using AutoMapper;
using LBS.Shared.Entity.BaseModels;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.SemiProductViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class SemiProductController : Controller
    {
        readonly ILogger<SemiProductController> _logger;
        readonly ISemiProductService _service;
        readonly IHttpClientService _httpClientService;
        readonly IMapper _mapper;
        readonly IProductTransactionLineService _transactionLineService;
        public SemiProductController(ILogger<SemiProductController> logger,
            IHttpClientService httpClientService,
            ISemiProductService service, IMapper mapper, IProductTransactionLineService transactionLineService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _mapper = mapper;
            _transactionLineService = transactionLineService;

        }


        public IActionResult Index()
        {
            ViewData["Title"] = "Yarı Mamuller";
            return View();
        }

        public async Task<IActionResult> Detail(int referenceId)
        {
            SemiProductDetailViewModel viewModel = new SemiProductDetailViewModel();
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var product = await _service.GetObject(httpClient, referenceId);

            if (httpClient == null)
                BadRequest();

            if (product == null)
                return NotFound();

            viewModel.SemiProductModel = _mapper.Map<SemiProductModel>(product);
            viewModel.SemiProductModel.SellQuentity = 0;
            viewModel.SemiProductModel.FirstQuentity = 0;
            viewModel.SemiProductModel.StockQuentity = 0;
            viewModel.SemiProductModel.PurchaseQuentity = 0;
            viewModel.SemiProductModel.RevolutionSpeed = 0;
            viewModel.SemiProductModel.DailyStock = 0;
            viewModel.SemiProductModel.DailyStockChange = 0;
            viewModel.SemiProductModel.WeeklyStock = 0;
            viewModel.SemiProductModel.WeeklyStockChange = 0;
            viewModel.SemiProductModel.MonthlyStock = 0;
            viewModel.SemiProductModel.MonthlyStockChange = 0;
            viewModel.SemiProductModel.YearlyStock = 0;
            viewModel.SemiProductModel.YearlyStockChange = 0;



            ViewData["Title"] = viewModel.SemiProductModel.Name;
            return View(viewModel);
        }
        public async ValueTask<IActionResult> GetJsonResult()
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

        public async ValueTask<IActionResult> GetWarehouseJsonResult(int productReferenceId)
        {
            //Console.WriteLine(productReferenceId.ToString());
            return Json(new { data = GetWarehouseSemiProduct(productReferenceId) });
        }


        public async IAsyncEnumerable<SemiProductListViewModel> GetSemiProduct()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _service.GetObjects(httpClient);

            await foreach (var item in result)
            {
                yield return new SemiProductListViewModel
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

        public async IAsyncEnumerable<ProductTransactionLine> GetInputSemiProduct(int productReferenceId)
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
        public async IAsyncEnumerable<ProductTransactionLine> GetOutputSemiProduct(int productReferenceId)
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

        public async IAsyncEnumerable<ProductTransactionLine> GetWarehouseSemiProduct(int productReferenceId)
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _transactionLineService.GetInputProductTransactionLineByProductRef(httpClient, productReferenceId);
            Console.WriteLine(productReferenceId.ToString());
            await foreach (var item in result)
            {
                Console.WriteLine(item.IOType.ToString());
                yield return item;
            }
        }
    }
}
