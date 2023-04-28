using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers;

public class ProductionOrderController : Controller
{

	readonly ILogger<ProductionOrderController> _logger;
	readonly IHttpClientService _httpClientService;
	IProductionService _productionService;
    public ProductionOrderController(ILogger<ProductionOrderController> logger,
		IHttpClientService httpClientService, IProductionService productionService)
    {
        _logger = logger;
		_httpClientService = httpClientService;
		_productionService = productionService;
    }
    public IActionResult Index()
	{
		return View();
	}

	[HttpPost]
	public async ValueTask<IActionResult> GetJsonResult()
	{
		return Json(new { data = GetProductions() });
	}

	private async IAsyncEnumerable<ProductionOrder> GetProductions()
	{
		HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();

		var result = _productionService.GetObjects(httpClient);

		await foreach (var item in result)
			yield return item;


	}

}
