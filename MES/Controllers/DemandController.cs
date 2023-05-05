using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
	public class DemandController : Controller
	{
		readonly ILogger<DemandController> _logger;
		readonly IDemandService _service;
		readonly IHttpClientService _httpClientService;
		public DemandController(ILogger<DemandController> logger,
			IHttpClientService httpClientService,
			IDemandService service)
		{
			_logger = logger;
			_httpClientService = httpClientService;
			_service = service;
		}


		public IActionResult Index()
		{
			ViewData["Title"] = "Bekleyen Talepler";
			return View();
		}

		public async ValueTask<IActionResult> GetJsonResult()
		{
			return Json(new { data = GetEmployees() });
		}


		public async IAsyncEnumerable<Demand> GetEmployees()
		{
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = _service.GetObjects(httpClient);

			await foreach (var item in result)
				yield return item;
		}
	}
}
