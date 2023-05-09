using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.RoutingViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
	public class RoutesController : Controller
	{
		ILogger<RoutesController> _logger;
		IHttpClientService _httpClientService;
		IRoutingService _service;


		public RoutesController(ILogger<RoutesController> logger,
			IHttpClientService httpClientService,
			IRoutingService service
			)
		{
			_logger = logger;
			_httpClientService = httpClientService;
			_service = service;

		}

		public IActionResult Index()
		{
			ViewData["Title"] = "Rotalar";
			RoutesListViewModel viewModel = new RoutesListViewModel();
			return View(viewModel);
		}

		[HttpPost]
		public async ValueTask<IActionResult> GetJsonResult()
		{
			return Json(new { data = GetOperation() });
		}

		public async IAsyncEnumerable<RoutesModel> GetOperation()
		{
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = _service.GetObjects(httpClient);


			await foreach (var item in result)
			{
				RoutesModel model = new RoutesModel
				{
					Code = item.Code,
					Name = item.Name,
					Active = item.Active,			
					ReferenceId = item.ReferenceId,
					CardType = item.CardType,
					Status = item.Status,
				};

				

				yield return model;
			}

		}


	}
}
