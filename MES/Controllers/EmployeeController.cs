using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
	public class EmployeeController : Controller
	{
		readonly ILogger<EmployeeController> _logger;
		readonly IEmployeeService _employeeService;
		readonly IHttpClientService _httpClientService;
        public EmployeeController(ILogger<EmployeeController> logger,
			IHttpClientService httpClientService,
			IEmployeeService employeeService)
        {
			_logger = logger;
			_httpClientService = httpClientService;
			_employeeService = employeeService;
        }


        public IActionResult Index()
		{
		
			return View();
		}

		public async ValueTask<IActionResult> GetJsonResult()
		{
			return Json(new { data = GetEmployees() });
		}


		public async IAsyncEnumerable<Employee> GetEmployees()
		{
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = _employeeService.GetObjects(httpClient);

			await foreach (var item in result)
				yield return item;
		}
			
	}
}
