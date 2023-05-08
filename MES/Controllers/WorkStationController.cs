using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.ProductViewModels;
using MES.ViewModels.WorkStationViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

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
            ViewData["Title"] = "İş İstasyonları";
            WorkStationListViewModel viewModel = new WorkStationListViewModel();
            return View(viewModel);
        }

        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetWorkstation() });
        }


        public async IAsyncEnumerable<WorkStationModel> GetWorkstation()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _service.GetObjects(httpClient);

            await foreach (var item in result)
            {
                yield return new WorkStationModel
                {
                    Code = item.Code,
                    Name = item.Name,
                    PermissionCode = item.PermissionCode,
                    ReferenceId = item.ReferenceId,
                    SpeCode = item.SpeCode,
                    EstimatedMaintanceDate = DateTime.Now
                    
                
                    
                };
            }
        }
    }
}
