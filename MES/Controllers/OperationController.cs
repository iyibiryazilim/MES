using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.OperationModels;
using MES.Models.ProductWarehouseParameterModels;
using MES.ViewModels.OperationViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers
{
    public class OperationController : Controller
    {
        readonly ILogger<OperationController> _logger;
        readonly IHttpClientService _httpClientService;
        readonly IOperationService _service;
        readonly IWorkOrderService _workOrderService;
        readonly ICustomQueryService _customQueryService;

        public OperationController(ILogger<OperationController> logger,
            IHttpClientService httpClientService,
            IOperationService service,
            IWorkOrderService workOrderService, ICustomQueryService customQueryService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _workOrderService = workOrderService;
            _customQueryService = customQueryService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Operasyonlar";
            return View();
        }

        [HttpPost]
        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetOperation() });
        }

        public async IAsyncEnumerable<OperationListModel> GetOperation()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            OperationListModel viewModel = new();

            if (viewModel != null)
            {
                const string query = @"
                    SELECT 
                    OPERATIONS.LOGICALREF AS [ReferenceId],
                    OPERATIONS.CODE AS [Code],
                    OPERATIONS.NAME AS [Description],
                    [ActiveWorkOrder] = (SELECT COUNT(*) FROM LG_003_DISPLINE WHERE OPERATIONREF = OPERATIONS.LOGICALREF)
                    FROM LG_003_OPERTION AS OPERATIONS ";

                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    List<OperationListModel> result = (List<OperationListModel>)jsonDocument.Deserialize(typeof(List<OperationListModel>), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (result != null)
                    {
                        foreach (OperationListModel item in result)
                        {

                            yield return item;
                        }
                    }
                }
            }
        }
    }
}
