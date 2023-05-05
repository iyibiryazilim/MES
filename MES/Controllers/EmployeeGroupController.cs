using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
	public class EmployeeGroupController : Controller
	{
		ILogger<EmployeeGroupController> _logger;
		IHttpClientService _httpClientService;
		IEmployeeGroupService _service;

        public EmployeeGroupController(ILogger<EmployeeGroupController> logger,
			IHttpClientService httpClientService,
			IEmployeeGroupService service)
        {
            _logger = logger;
			_httpClientService = httpClientService;
			_service = service;

        }

        public IActionResult Index()
		{
            ViewData["Title"] = "Çalışan Grupları";
            return View();
		}

		[HttpPost]
		public async ValueTask<IActionResult> GetJsonResult()
		{
			return Json(new { data = GetEmployeGroups() });
		}

		public async IAsyncEnumerable<EmployeeGroup> GetEmployeGroups()
		{
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = _service.GetObjects(httpClient);

			await foreach (var item in result)			
				yield return item;
			
		}
	}
}
