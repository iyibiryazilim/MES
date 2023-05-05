using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class ShiftController : Controller
    {
        readonly ILogger<ShiftController> _logger;
        readonly IHttpClientService _httpClientService;
        readonly IShiftService _service;

        public ShiftController(ILogger<ShiftController> logger,
            IHttpClientService httpClientService,
            IShiftService service)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Vardiyalar";
            return View();
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetShifts() });

        }


        public async IAsyncEnumerable<Shift> GetShifts()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _service.GetObjects(httpClient);

            await foreach (var item in result)
            {
                yield return item;
            }
        }
    }
}
