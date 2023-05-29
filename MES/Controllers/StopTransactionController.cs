using System.Text.Json;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.Models.StopCauseModels;
using MES.Models.StopTransactionModels;
using MES.ViewModels.StopTransactionViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class StopTransactionController : Controller
    {
        ILogger<StopTransactionController> _logger;
        IHttpClientService _httpClientService;
        IStopTransactionService _service;
        ICustomQueryService _customQueryService;


        public StopTransactionController(ILogger<StopTransactionController> logger,
            IHttpClientService httpClientService,
            IStopTransactionService service,
            ICustomQueryService customQueryService
            )
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _customQueryService = customQueryService;

        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Duruş Hareketleri";
            StopTransactionListViewModel viewModel = new StopTransactionListViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetOperation() });
        }

        public async IAsyncEnumerable<StopTransactionListModel> GetOperation()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            StopTransactionListModel viewModel = new();

            if (viewModel != null)
            {
                const string query = @"SELECT 
                STOPTRANS.LOGICALREF AS [ReferenceId],
                STOPTRANS.STOPDATE AS [StopDate],
                dbo.LG_INTTOTIME(STOPTRANS.STOPTIME) AS [StopTime],
                STOPTRANS.STARTDATE AS [StartDate],
                dbo.LG_INTTOTIME(STOPTRANS.STARTTIME) AS [StartTime],
                STOPTRANS.STOPDURATION AS [StopDuration],
                OPERATION.LOGICALREF AS [OperationReferenceId],
                OPERATION.CODE AS [OperationCode],
                OPERATION.NAME AS [OperationName],
                PRODUCTIONORDER.LOGICALREF AS [ProductionOrderReferenceId],
                PRODUCTIONORDER.FICHENO AS [ProductionOrderCode],
                WORKORDER.LOGICALREF AS [WorkOrderReferenceId],
                WORKORDER.WFSTATUS AS [WorkOrderStatus],
                WORKSTAT.LOGICALREF AS [WorkstationReferenceId],
                WORKSTAT.CODE AS [WorkstationCode],
                WORKSTAT.NAME AS [WorkstationName],
                STOPCAUSE.LOGICALREF AS [StopCauseReferenceId],
                STOPCAUSE.CODE AS [StopCauseCode],
                STOPCAUSE.NAME AS [StopCauseName]
                FROM LG_003_STOPTRANS AS STOPTRANS
                LEFT JOIN LG_003_WORKSTAT AS WORKSTAT ON WORKSTAT.LOGICALREF = STOPTRANS.WSREF
                LEFT JOIN LG_003_DISPLINE AS WORKORDER ON WORKORDER.LOGICALREF = STOPTRANS.DISPLINEREF
                LEFT JOIN LG_003_PRODORD AS PRODUCTIONORDER ON PRODUCTIONORDER.LOGICALREF = STOPTRANS.PRODORDREF
                LEFT JOIN LG_003_OPERTION AS OPERATION ON OPERATION.LOGICALREF = STOPTRANS.OPREF
                LEFT JOIN LG_003_STOPCAUSE AS STOPCAUSE ON STOPCAUSE.LOGICALREF = STOPTRANS.CAUSEREF";
                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<StopTransactionListModel> result = (List<StopTransactionListModel>)jsonDocument.Deserialize(typeof(List<StopTransactionListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (StopTransactionListModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }


        }
    }
}
