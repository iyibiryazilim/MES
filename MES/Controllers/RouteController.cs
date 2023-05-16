using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.Models;
using MES.ViewModels.RoutingViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers
{
	public class RouteController : Controller
	{
		ILogger<RouteController> _logger;
		IHttpClientService _httpClientService;
		IRouteService _service;


		public RouteController(ILogger<RouteController> logger,
			IHttpClientService httpClientService,
            IRouteService service
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
		
		public async IAsyncEnumerable<RouteModel> GetOperation()
		{
			HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = _service.GetObjects(httpClient);


			await foreach (var item in result)
			{ 
				RouteModel model = new RouteModel
				{
					Active = item.Active,
					CardType = item.CardType,
					Code = item.Code,
					Name = item.Name,
					ReferenceId = item.ReferenceId,
					Status = item.Status
					
				};

				
				
				yield return model;
			}

		}


	}
}
