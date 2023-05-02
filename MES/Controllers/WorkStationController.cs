using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.ViewModels.ProductViewModels;
using MES.ViewModels.WorkStationViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class WorkStationController : Controller
    {
        readonly ILogger<WorkStationController> _logger;
        readonly IWorkstationServise _service;
        readonly IHttpClientService _httpClientService;
        public WorkStationController(ILogger<WorkStationController> logger,
            IHttpClientService httpClientService,
            IWorkstationServise service)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
        }


        public IActionResult Index()
        {

            return View();
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetWorkstation() });
        }


        public async IAsyncEnumerable<WorkStationListViewModel> GetWorkstation()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _service.GetObjects(httpClient);

            await foreach (var item in result)
            {
                yield return new WorkStationListViewModel
                {
                    Code = item.Code,
                    Name = item.Name,
                    PermissionCode = item.PermissionCode,
                    ReferenceId = item.ReferenceId,
                    SpeCode = item.SpeCode
                };
            }
        }
    }
}
