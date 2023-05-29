using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.BOM;
using MES.Models.OperationModels;
using MES.ViewModels.OperationViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MES.Controllers
{
    public class BOMController : Controller
    {
        readonly ILogger<BOMController> _logger;
        readonly IHttpClientService _httpClientService;
        readonly IBOMService _service;
        readonly ICustomQueryService _customQueryService;

        public BOMController(ILogger<BOMController> logger,
            IHttpClientService httpClientService,
            IBOMService service, ICustomQueryService customQueryService)
        {
            _logger = logger;
            _httpClientService = httpClientService;
            _service = service;
            _customQueryService = customQueryService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Reçeteler";
            return View();
        }

        [HttpPost]
        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetBOM() });
        }

        public async IAsyncEnumerable<BOMListModel> GetBOM()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            BOMListModel viewModel = new();

            if (viewModel != null)
            {
                const string query = @"
                    SELECT BOMASTER.LOGICALREF AS [REFERENCEID],
                    BOMASTER.CODE AS [CODE],
                    BOMASTER.NAME AS [NAME],
                    [RevisionDate] = (SELECT REVDATE FROM LG_003_BOMREVSN WHERE LOGICALREF = BOMASTER.VALIDREVREF),
                    ITEMS.CODE AS [PRODUCTCODE],
                    ITEMS.NAME AS [PRODUCTNAME],
                    ITEMS.LOGICALREF AS [PRODUCTREFERENCEID]
                    FROM LG_003_BOMASTER AS BOMASTER
                    LEFT JOIN LG_003_ITEMS AS ITEMS ON BOMASTER.MAINPRODREF = ITEMS.LOGICALREF ";

                JsonDocument? jsonDocument = await _customQueryService.GetObjects(httpClient, query);
                if (jsonDocument != null)
                {
                    var array = jsonDocument.RootElement.EnumerateArray();
                    foreach (JsonElement element in array)
                    {
                        #region Reference Id
                        JsonElement referenceId = element.GetProperty("referenceid");
                        viewModel.ReferenceId = referenceId.GetInt32();
                        #endregion

                        #region Code
                        JsonElement code = element.GetProperty("code");
                        viewModel.Code = code.GetString();
                        #endregion

                        #region Name
                        JsonElement name = element.GetProperty("name");
                        viewModel.Name = name.GetString();
                        #endregion

                        #region Product Reference Id
                        JsonElement productReferenceId = element.GetProperty("referenceid");
                        viewModel.ProductReferenceId = productReferenceId.GetInt32();
                        #endregion

                        #region Product Code
                        JsonElement productCode = element.GetProperty("code");
                        viewModel.ProductCode = productCode.GetString();
                        #endregion

                        #region Product Name
                        JsonElement productName = element.GetProperty("name");
                        viewModel.ProductName = productName.GetString();
                        #endregion

                        #region Revision Date
                        JsonElement revisionDate = element.GetProperty("revisionDate");
                        if (element.TryGetProperty("plannedbegin", out revisionDate) && revisionDate.ValueKind != JsonValueKind.Null)
                        {
                            viewModel.RevisionDate = JsonSerializer.Deserialize<DateTime>(revisionDate.GetRawText());
                        }
                        #endregion

                        yield return viewModel;
                    }
                }
            }
        }
    }
}
