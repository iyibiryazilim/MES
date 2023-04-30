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
		IEmployeeGroupService _employeeGroupService;

        public EmployeeGroupController(ILogger<EmployeeGroupController> logger,
			IHttpClientService httpClientService,
			IEmployeeGroupService employeeGroupService)
        {
            _logger = logger;
			_httpClientService = httpClientService;
			_employeeGroupService = employeeGroupService;

        }

        public IActionResult Index()
		{
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
			var result = _employeeGroupService.GetObjects(httpClient);

			await foreach (var item in result)			
				yield return item;
			
		}
	}
}
