using System.Text.Json;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models.RouteModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
    public class RouteController : Controller
    {
        readonly ILogger<RouteController> _logger;
        readonly IHttpClientService _httpClientService;
        readonly IRouteService _service;
        readonly ICustomQueryService _customQueryService;

        public RouteController(ILogger<RouteController> logger,
            IHttpClientService httpClientService,
            IRouteService service,
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
            ViewData["Title"] = "Rotalar";
            return View();
        }

        [HttpPost]
        public async ValueTask<IActionResult> GetJsonResult()
        {
            return Json(new { data = GetOperation() });
        }

        public async IAsyncEnumerable<RouteListModel> GetOperation()
        {
            HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
            RouteListModel viewModel = new();

            if (viewModel != null)
            {
                const string query = @"SELECT 
					  ROUTING.LOGICALREF AS [REFERENCEID], 
					  ROUTING.CODE AS [CODE], 
					  ROUTING.WFSTATUS AS [STATUS], 
					  ROUTING.NAME AS [DESCRIPTION], 
					  ROUTING.CARDTYPE AS [CARDTYPE] 
					FROM 
					  LG_003_ROUTING AS ROUTING";
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

                        #region Code
                        JsonElement code = element.GetProperty("code");
                        viewModel.Code = code.GetString();
                        #endregion

                        #region Description
                        JsonElement description = element.GetProperty("description");
                        viewModel.Description = description.GetString();
                        #endregion

						#region CardType
                        JsonElement cardType = element.GetProperty("cardtype");
                        viewModel.CardType = (short)Convert.ToInt32(cardType.GetRawText().Replace('.', ','));
                        #endregion

						#region Status
                        JsonElement status = element.GetProperty("status");
						var value = false;
						if(Convert.ToInt32(status.GetRawText().Replace('.', ',')) == 1)
						{
							value = true;
						}
                        viewModel.Status = value;
                        #endregion

                        yield return viewModel;
                    }
                }
            }

        }


    }
}
