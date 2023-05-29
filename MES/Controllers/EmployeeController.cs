using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.EmployeeGroupModels;
using MES.Models.EmployeeModels;
using MES.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers
{
    public class EmployeeController : Controller
    {
        readonly ILogger<EmployeeController> _logger;
        readonly IEmployeeService _employeeService;
        readonly IHttpClientService _httpClientService;
        readonly ICustomQueryService _customQueryService;
        public EmployeeController(ILogger<EmployeeController> logger,
            IHttpClientService httpClientService,
            IEmployeeService employeeService, ICustomQueryService customQueryService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _employeeService = employeeService;
            _customQueryService = customQueryService;
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


        public async IAsyncEnumerable<EmployeeListModel> GetEmployees()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            EmployeeListModel viewModel = new();

            if (viewModel != null)
            {
                const string query = @"SELECT EMPLOYEE.LOGICALREF AS [ReferenceId],
                EMPLOYEE.NAME AS [Code],
                EMPLOYEE.CODE AS [Name]
                FROM LG_003_EMPLOYEE AS EMPLOYEE";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<EmployeeListModel> result = (List<EmployeeListModel>)jsonDocument.Deserialize(typeof(List<EmployeeListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (EmployeeListModel item in result)
                        {
                            item.ShiftRate = 0;
                            yield return item;
                        }
                    }
                }
            }
        }

    }
}
