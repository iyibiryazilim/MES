using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class OperationController : Controller
    {
        ILogger<OperationController> _logger;
        IHttpClientService _httpClientService;
        IOperationService _service;

        public OperationController(ILogger<OperationController> logger,
            IHttpClientService httpClientService,
            IOperationService service)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;

        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Operasyonlar";
            return View();
        }

        [HttpPost]
        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetOperation() });
        }

        public async IAsyncEnumerable<Operation> GetOperation()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _service.GetObjects(httpClient);

            await foreach (var item in result)
                yield return item;

        }
    }
}
