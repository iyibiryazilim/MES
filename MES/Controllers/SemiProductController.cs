using LBS.Shared.Entity.BaseModels;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
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
        public SemiProductController(ILogger<SemiProductController> logger,
            IHttpClientService httpClientService,
            ISemiProductService service)
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
