using System;
using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.SemiProductViewModels;
using MES.ViewModels.WorkOrderViewModels;
using MES.ViewModels.WorkStationViewModels;
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
        WorkOrderListViewModel viewModel = new WorkOrderListViewModel();
        return View(viewModel);
    }

    [HttpPost]
    public async ValueTask<IActionResult> GetJsonResult()
    {
        return Json(new { data = GetWorkOrder() });
    }

    private async IAsyncEnumerable<WorkOrderModel> GetWorkOrder()
    {

        string query = "SELECT  WORKSTATION.NAME AS [WORKSTATIONNAME], WORKORDER.WSREF AS [WORKSTATIONID]," +
            "\nITEMS.NAME AS [PRODUCTNAME],WORKORDER.ITEMREF AS [PRODUCTID]," +
            "\nWORKORDER.LINESTATUS AS [WORKORDERSTATUS],WORKORDER.LOGICALREF AS [REFERENCEID]," +
            "\nWORKORDER.OPBEGDATE AS [PLANNEDBEGIN],OPERATION.LOGICALREF AS [OPERATIONID]," +
            "\nOPERATION.NAME AS [OPERATIONNAME],WORKORDER.OPDUEDATE AS [PLANNEDDUE]," +
            "\nPRODUCTIONORDER.PLNAMOUNT AS [PLANNEDAMOUNT]," +
            "\n[ActualAmmount]=(SELECT ISNULL(SUM(AMOUNT),0) FROM LG_003_01_STLINE" +
            "\nWHERE POLINEREF = WORKORDER.LOGICALREF AND TRCODE = 13 AND LPRODSTAT = 1)" +
            "\nFROM LG_003_DISPLINE AS WORKORDER" +
            "\nLEFT JOIN LG_003_WORKSTAT AS WORKSTATION ON WORKORDER.WSREF = WORKSTATION.LOGICALREF" +
            "\nLEFT JOIN LG_003_ITEMS AS ITEMS ON WORKORDER.ITEMREF = ITEMS.LOGICALREF" +
            "\nLEFT JOIN LG_003_OPERTION AS OPERATION ON WORKORDER.OPERATIONREF = OPERATION.LOGICALREF" +
            "\nLEFT JOIN LG_003_PRODORD AS PRODUCTIONORDER ON WORKORDER.PRODORDREF = PRODUCTIONORDER.LOGICALREF\n\n";

        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();

        var result = _workOrderService.Getobjects(httpClient);
        await foreach (var item in result)
        {
            WorkOrderModel _workOrder = new WorkOrderModel
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
                RealizationRate = 50,

            };
            //_workOrder.RealizationRate = Convert.ToInt32((_workOrder.ActualAmount / _workOrder.PlannedAmount) * 100);
            yield return _workOrder;

        }



    }
}

