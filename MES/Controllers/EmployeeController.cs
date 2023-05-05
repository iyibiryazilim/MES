using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.ViewModels.EmployeeGroupViewModels;
using MES.ViewModels.EmployeeViewModels;
using MES.ViewModels.ProductViewModels;
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
            ViewData["Title"] = "Çalışanlar";
            return View();
		}

		public async ValueTask<IActionResult> GetJsonResult()
		{
			return Json(new { data = GetEmployees() });
		}


		public async IAsyncEnumerable<EmployeeListViewModel> GetEmployees()
		{
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = _employeeService.GetObjects(httpClient);

			await foreach (var item in result)
			{
				yield return new EmployeeListViewModel
				{
					Code = item.Code,
					Name = item.Name,
					ReferenceId = item.ReferenceId,
					ShiftProgress = 67
				};
			}
		}
			
	}
}
