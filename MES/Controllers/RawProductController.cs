using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.ViewModels.ProductViewModels;
using MES.ViewModels.RawProductViewModel;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class RawProductController : Controller
    {
        readonly ILogger<RawProductController> _logger;
        readonly IRawProductService _service;
        readonly IHttpClientService _httpClientService;
        public RawProductController(ILogger<RawProductController> logger,
            IHttpClientService httpClientService,
            IRawProductService service)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
        }


        public IActionResult Index()
        {

            return View();
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetRawProduct() });
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
    }
}
