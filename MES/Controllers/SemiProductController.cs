using AutoMapper;
using LBS.Shared.Entity.BaseModels;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.ProductViewModels;
using MES.ViewModels.RawProductViewModel;
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
        public SemiProductController(ILogger<SemiProductController> logger,
            IHttpClientService httpClientService,
            ISemiProductService service, IMapper mapper)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _mapper = mapper;
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



            ViewData["Title"] = viewModel.SemiProductModel.Name;
            return View(viewModel);
        }
        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetSemiProduct() });
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
    }
}
