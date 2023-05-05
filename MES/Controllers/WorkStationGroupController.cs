using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers;

public class WorkStationGroupController : Controller
{
	readonly ILogger<WorkStationGroupController> _logger;
	readonly IHttpClientService _httpClientService;
	readonly IWorkstationGroupService _workstationGroupService;
	public WorkStationGroupController(ILogger<WorkStationGroupController> logger,
		IHttpClientService httpClientService,
		IWorkstationGroupService workstationGroupService)
	{
		_logger = logger;
		_httpClientService = httpClientService;
		_workstationGroupService = workstationGroupService;
	}
	public IActionResult Index()
	{
        ViewData["Title"] = "İş İstasyonu Grupları";
        return View();
	}

	[HttpPost]
	public async ValueTask<IActionResult> GetJsonResult()
	{
		return Json(new { data = GetWorkstations() });
	}

	private async IAsyncEnumerable<WorkstationGroup> GetWorkstations()
	{
		HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();

		var result = _workstationGroupService.GetObjects(httpClient);

		await foreach (var item in result)
			yield return item;


	}
}
