using System;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.ViewModels.SemiProductViewModels;
using MES.ViewModels.WorkOrderViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers;

public class WorkOrderController : Controller
{
    readonly ILogger<WorkOrderController> _logger;
    readonly IHttpClientService _httpClientService;
    readonly IWorkOrderService _workOrderService;
    public WorkOrderController(ILogger<WorkOrderController> logger,
        IHttpClientService httpClientService,
        IWorkOrderService workOrderService)
    {
        _logger = logger;
        _httpClientService = httpClientService;
        _workOrderService = workOrderService;
    }
    public IActionResult Index()
    {
        ViewData["Title"] = "İş Emirleri";
        return View();
    }

    [HttpPost]
    public async ValueTask<IActionResult> GetJsonResult()
    {
        return Json(new { data = GetWorkOrder() });
    }

    private async IAsyncEnumerable<WorkOrderListViewModel> GetWorkOrder()
    {
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();

        var result = _workOrderService.Getobjects(httpClient);
        await foreach (var item in result)
        {
            WorkOrderListViewModel _workOrder = new WorkOrderListViewModel
            {
                Operation = item.Operation,
                OperationActualBeginDate = item.OperationActualBeginDate,
                OperationActualDueDate = item.OperationActualDueDate,
                OperationBeginDate = item.OperationBeginDate,
                OperationDueDate = item.OperationDueDate,
                Product = item.Product,
                ReferenceId = item.ReferenceId,
                Status = item.Status,
                Workstation = item.Workstation,
                ActualAmount = 0,
                PlannedAmount = 0,
                RealizationRate = 10,
                
            };
            //_workOrder.RealizationRate = Convert.ToInt32((_workOrder.ActualAmount / _workOrder.PlannedAmount) * 100);
            yield return _workOrder;
            
        }
           


    }
}

