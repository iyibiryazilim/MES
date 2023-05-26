using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.WorkOrderModels;
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
        IWorkOrderService workOrderService,ICustomQueryService customQueryService)
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
			string query = $@"SELECT WORKSTATION.NAME AS[WORKSTATIONNAME]
							 , WORKSTATION.CODE AS [WORKSTATIONCODE]
                             , WORKORDER.WSREF AS[WORKSTATIONID]
                             , ITEMS.NAME AS [PRODUCTNAME]
							 , ITEMS.CODE AS [PRODUCTCODE]
                             , WORKORDER.ITEMREF AS[PRODUCTID]
                             , WORKORDER.LINESTATUS AS[WORKORDERSTATUS]
                             , WORKORDER.LOGICALREF AS[REFERENCEID]
                             , WORKORDER.OPBEGDATE AS[PLANNEDBEGIN]
                             , OPERATION.LOGICALREF AS[OPERATIONID]
                             , OPERATION.NAME AS [OPERATIONNAME]
                             , WORKORDER.OPDUEDATE AS [PLANNEDDUE]
                             , PRODUCTIONORDER.PLNAMOUNT AS[PLANNEDAMOUNTH]
                             ,[ActualAmounth] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_003_01_STLINE WHERE POLINEREF = WORKORDER.LOGICALREF AND TRCODE = 13 AND LPRODSTAT = 1)
							 
                             FROM LG_003_DISPLINE AS WORKORDER LEFT JOIN LG_003_WORKSTAT AS WORKSTATION ON WORKORDER.WSREF = WORKSTATION.LOGICALREF
                             LEFT JOIN LG_003_ITEMS AS ITEMS ON WORKORDER.ITEMREF = ITEMS.LOGICALREF
                             LEFT JOIN LG_003_OPERTION AS OPERATION ON WORKORDER.OPERATIONREF = OPERATION.LOGICALREF
                             LEFT JOIN LG_003_PRODORD AS PRODUCTIONORDER ON WORKORDER.PRODORDREF = PRODUCTIONORDER.LOGICALREF";
			JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
			if (jsonDocument != null)
			{
				var array = jsonDocument.RootElement.EnumerateArray();
				foreach (JsonElement element in array)
				{
					#region Reference Id
					JsonElement referenceId = element.GetProperty("referenceid");
					viewModel.ReferenceId = Convert.ToInt32(referenceId.GetRawText().Replace('.', ','));
					#endregion

					#region Product Name
					JsonElement productName = element.GetProperty("productname");
					viewModel.ProductName = productName.GetString();
					#endregion

					#region Product Code
					JsonElement productCode = element.GetProperty("productcode");
					viewModel.ProductCode = productCode.GetString();
					#endregion

					#region Product Id
					JsonElement productId = element.GetProperty("productid");
					viewModel.ProductId = Convert.ToInt32(productId.GetRawText().Replace('.', ','));
					#endregion

					#region Workstation Name 
					JsonElement workstationName = element.GetProperty("workstationname");
					viewModel.WorkstationName = workstationName.GetString();
					#endregion

					#region Workstation Id
					JsonElement workstationId = element.GetProperty("workstationid");
					viewModel.WorkstationId = Convert.ToInt32(workstationId.GetRawText().Replace('.', ','));
					#endregion

					#region Workstation Code 
					JsonElement workstationCode = element.GetProperty("workstationcode");
					viewModel.WorkstationCode = workstationCode.GetString();
					#endregion

					#region Work Order Status
					JsonElement workOrderStatus = element.GetProperty("workorderstatus");
					viewModel.WorkOrderStatus = Convert.ToInt32(workOrderStatus.GetRawText().Replace('.', ','));
					#endregion

					#region Operation Id 
					JsonElement operationId = element.GetProperty("operationid");
					viewModel.OperationId = Convert.ToInt32(operationId.GetRawText().Replace('.', ','));
					#endregion

					#region  Operation Name
					JsonElement operationName = element.GetProperty("operationname");
					viewModel.OperationName = operationName.GetString();
					#endregion

					#region Planned Begin
					JsonElement plannedBeginDate = element.GetProperty("plannedbegin");
					if (element.TryGetProperty("plannedbegin", out plannedBeginDate) && plannedBeginDate.ValueKind != JsonValueKind.Null)
					{
						viewModel.PlannedBeginDate = JsonSerializer.Deserialize<DateTime>(plannedBeginDate.GetRawText());
					}
					#endregion

					#region Planned Due
					JsonElement plannedDueDate = element.GetProperty("planneddue");
					if (element.TryGetProperty("planneddue", out plannedDueDate) && plannedDueDate.ValueKind != JsonValueKind.Null)
					{
						viewModel.PlannedDueDate = JsonSerializer.Deserialize<DateTime>(plannedDueDate.GetRawText());
					}
					#endregion

					#region  Planned Amounth
					JsonElement plannedAmounth = element.GetProperty("plannedamounth");
					viewModel.PlannedAmounth = Convert.ToDouble(plannedAmounth.GetRawText().Replace('.', ','));
					#endregion

					#region  Actual Amounth
					JsonElement actualAmounth = element.GetProperty("actualAmounth");
					viewModel.ActualAmounth = Convert.ToDouble(actualAmounth.GetRawText().Replace('.', ','));
					#endregion


					//Console.WriteLine(Convert.ToInt64(100 * (viewModel.ActualAmounth / viewModel.PlannedAmounth)).ToString());
					#region Realization Rate
					viewModel.RealizationRate = Convert.ToInt64(100*(viewModel.ActualAmounth / viewModel.PlannedAmounth));
					#endregion

					yield return viewModel;
				}
			}
		}
	}
}

