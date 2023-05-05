using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class SalesOrderLineController : Controller
    {
        readonly ILogger<SalesOrderLineController> _logger;
        readonly IHttpClientService _httpClientService;
        readonly ISalesOrderLineService _salesOrderLineService;

        public SalesOrderLineController(ILogger<SalesOrderLineController> logger,
             IHttpClientService httpClientService,
             ISalesOrderLineService salesOrderLineService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _salesOrderLineService = salesOrderLineService;
            
        }
        public IActionResult Index()
        {

            return View();
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetSalesOrders() });        
                
        }


        public async IAsyncEnumerable<SalesOrderLine> GetSalesOrders()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _salesOrderLineService.GetObjects(httpClient);

            await foreach (var item in result)
            {
                yield return item;
            }
        }

    }
}
