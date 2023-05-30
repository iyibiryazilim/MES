using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.WorkOrderModels;
using MES.Models.WorkstationModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers;

public class WorkOrderController : Controller
{
    readonly ILogger<WorkOrderController> _logger;
    readonly IHttpClientService _httpClientService;
    readonly IWorkOrderService _workOrderService;
    readonly ICustomQueryService _customQueryService;
    public WorkOrderController(ILogger<WorkOrderController> logger,
        IHttpClientService httpClientService,
        IWorkOrderService workOrderService, ICustomQueryService customQueryService)
    {
        _logger = logger;
        _httpClientService = httpClientService;
        _workOrderService = workOrderService;
        _customQueryService = customQueryService;
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

    private async IAsyncEnumerable<WorkOrderListModel> GetWorkOrder()
    {
        WorkOrderListModel viewModel = new WorkOrderListModel();
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
        if (viewModel != null)
        {
            string query = $@"SELECT WORKSTATION.NAME AS[WorkstationName]
							 , WORKSTATION.CODE AS [WorkstationCode]
                             , WORKORDER.WSREF AS[WorkstationId]
                             , ITEMS.NAME AS [ProductName]
							 , ITEMS.CODE AS [ProductCode]
                             , WORKORDER.ITEMREF AS [ProductId]
                             , WORKORDER.LINESTATUS AS[WORKORDERSTATUS]
                             , WORKORDER.LOGICALREF AS[ReferenceId]
                             , WORKORDER.OPBEGDATE AS[PlannedBeginDate]
                             , OPERATION.LOGICALREF AS[OperationId]
                             , OPERATION.NAME AS [OperationName]
                             , WORKORDER.OPDUEDATE AS [PlannedDueDate]
                             , PRODUCTIONORDER.PLNAMOUNT AS[PlannedAmounth]
                             ,[ActualAmounth] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_003_01_STLINE WHERE POLINEREF = WORKORDER.LOGICALREF AND TRCODE = 13 AND LPRODSTAT = 1)
							 
                             FROM LG_003_DISPLINE AS WORKORDER LEFT JOIN LG_003_WORKSTAT AS WORKSTATION ON WORKORDER.WSREF = WORKSTATION.LOGICALREF
                             LEFT JOIN LG_003_ITEMS AS ITEMS ON WORKORDER.ITEMREF = ITEMS.LOGICALREF
                             LEFT JOIN LG_003_OPERTION AS OPERATION ON WORKORDER.OPERATIONREF = OPERATION.LOGICALREF
                             LEFT JOIN LG_003_PRODORD AS PRODUCTIONORDER ON WORKORDER.PRODORDREF = PRODUCTIONORDER.LOGICALREF";
            JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
            if (jsonDocument != null)
            {
                List<WorkOrderListModel> result = (List<WorkOrderListModel>)jsonDocument.Deserialize(typeof(List<WorkOrderListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (result != null)
                {
                    foreach (WorkOrderListModel item in result)
                    {

                        yield return item;
                    }
                }
            }
        }
    }
}

