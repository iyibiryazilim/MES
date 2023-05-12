using AutoMapper;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class EndProductController : Controller
    {
        readonly ILogger<EndProductController> _logger;
        readonly IEndProductService _service;
        readonly IHttpClientService _httpClientService;
        readonly IMapper _mapper;
        public EndProductController(ILogger<EndProductController> logger,
            IHttpClientService httpClientService,
            IEndProductService service, IMapper mapper)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _mapper = mapper;

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
            viewModel.EndProductModel.DailyStock = 0;
            viewModel.EndProductModel.DailyStockChange = 0;
            viewModel.EndProductModel.WeeklyStock = 0;
            viewModel.EndProductModel.WeeklyStockChange = 0;
            viewModel.EndProductModel.MonthlyStock = 0;
            viewModel.EndProductModel.MonthlyStockChange = 0;
            viewModel.EndProductModel.YearlyStock = 0;
            viewModel.EndProductModel.YearlyStockChange = 0;

            ViewData["Title"] = viewModel.EndProductModel.Name;
            return View(viewModel);
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetEndProducts() });
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
    }
}
