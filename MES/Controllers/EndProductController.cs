using LBS.Shared.Entity.BaseModels;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class EndProductController : Controller
    {
        readonly ILogger<EndProductController> _logger;
        readonly IEndProductService _service;
        readonly IHttpClientService _httpClientService;
        public EndProductController(ILogger<EndProductController> logger,
            IHttpClientService httpClientService,
            IEndProductService service)
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
