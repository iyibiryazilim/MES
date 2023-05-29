using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.ShiftModels;
using MES.Models.WorkOrderModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers
{
    public class ShiftController : Controller
    {
        readonly ILogger<ShiftController> _logger;
        readonly IHttpClientService _httpClientService;
        readonly IShiftService _service;
        readonly ICustomQueryService _customQueryService;
        public ShiftController(ILogger<ShiftController> logger,
            IHttpClientService httpClientService,
            IShiftService service, ICustomQueryService customQueryService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _customQueryService = customQueryService;
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


        public async IAsyncEnumerable<ShiftListModel> GetShifts()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            ShiftListModel viewModel = new();

            if (viewModel != null)
            {
                const string query = @"SELECT SHIFT_.LOGICALREF AS [ReferenceId],
					SHIFT_.CODE AS [Code],
					SHIFT_.NAME AS [Name]
					FROM LG_003_SHIFT AS SHIFT_";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<ShiftListModel> result = (List<ShiftListModel>)jsonDocument.Deserialize(typeof(List<ShiftListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (ShiftListModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }
        }
    }
}
