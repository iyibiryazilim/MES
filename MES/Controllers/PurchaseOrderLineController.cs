using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class PurchaseOrderLineController : Controller
    {
        readonly ILogger<PurchaseOrderLineController> _logger;
        readonly IHttpClientService _httpClientService;
        readonly IPurchaseOrderLineService _purchaseOrderLineService;

        public PurchaseOrderLineController(ILogger<PurchaseOrderLineController> logger, 
            IHttpClientService httpClientService,
            IPurchaseOrderLineService purchaseOrderLineService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _purchaseOrderLineService = purchaseOrderLineService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetSalesOrders() });

        }


        public async IAsyncEnumerable<PurchaseOrderLine> GetSalesOrders()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _purchaseOrderLineService.GetObjects(httpClient);

            await foreach (var item in result)
            {
                yield return item;
            }
        }
    }
}
