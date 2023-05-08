using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.OperationViewModels;
using MES.ViewModels.WorkOrderViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MES.Controllers
{
	public class OperationController : Controller
    {
        ILogger<OperationController> _logger;
        IHttpClientService _httpClientService;
        IOperationService _service;
        IWorkOrderService _workOrderService;

        public OperationController(ILogger<OperationController> logger,
            IHttpClientService httpClientService,
            IOperationService service,
            IWorkOrderService workOrderService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _workOrderService = workOrderService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Operasyonlar";
            OperationListViewModel viewModel = new OperationListViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetOperation() });
        }

        public async IAsyncEnumerable<OperationModel> GetOperation()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = _service.GetObjects(httpClient);
            

			await foreach (var item in result)
			{
                OperationModel model = new OperationModel
                {
                    Active = item.Active,
                    Code = item.Code,
                    Name = item.Name,
                    ReferenceId = item.ReferenceId,
                };

                model.ActiveWorkOrderCount = 0;
                var workOrderResult = _workOrderService.Getobjects(httpClient);
                await foreach(var workOrderItem in workOrderResult)
                {
                    if(workOrderItem.Operation.ReferenceId == item.ReferenceId)
                    {
                        model.ActiveWorkOrderCount++;
                    }
                }

                yield return model;
            }

        }

        
        
    }
}
