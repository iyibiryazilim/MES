using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class StopCauseController : Controller
    {
        readonly ILogger<StopCauseController> _logger;
        readonly IStopCauseService _stopCauseService;
        readonly IHttpClientService _httpClientService;

        public StopCauseController(ILogger<StopCauseController> logger,
            IStopCauseService stopCauseService,
            IHttpClientService httpClientService)
        {
            _logger = logger;
            _stopCauseService = stopCauseService;
            _httpClientService = httpClientService;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async ValueTask<IActionResult> GetJsonResult() 
        {
            return Json(new { data = GetStopCauses() });
        }

        public async IAsyncEnumerable<StopCause> GetStopCauses()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _stopCauseService.GetObjects(httpClient);

            await foreach (var item in result)
            {
                yield return item;
            }
        }
    }
}
