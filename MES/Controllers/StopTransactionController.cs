using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.StopTransactionViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
	public class StopTransactionController : Controller
	{
		ILogger<StopTransactionController> _logger;
		IHttpClientService _httpClientService;
		IStopTransactionService _service;


		public StopTransactionController(ILogger<StopTransactionController> logger,
			IHttpClientService httpClientService,
			IStopTransactionService service
			)
		{
			_logger = logger;
			_httpClientService = httpClientService;
			_service = service;

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

		public async IAsyncEnumerable<StopTransactionModel> GetOperation()
		{
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = _service.GetObjects(httpClient);


			await foreach (var item in result)
			{
				StopTransactionModel model = new StopTransactionModel
				{
					Description = item.Description,
					Operation = item.Operation,
					ProductionOrder = item.ProductionOrder,
					ReferenceId = item.ReferenceId,
					StartDate = item.StartDate,
					StartTime = item.StartTime,
					StopCause = item.StopCause,
					StopDate = item.StopDate,
					StopDuration = item.StopDuration,
					StopTime = item.StopTime,
					WorkOrder = item.WorkOrder,
					Workstation = item.Workstation

				};



				yield return model;
			}

		}
	}
}
