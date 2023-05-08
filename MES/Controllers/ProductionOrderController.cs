using LBS.Shared.Entity.Models;
using LBS.WebAPI.Service.Services;
using MES.HttpClientService;
using MES.ViewModels.OperationOrderViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MES.Controllers;

public class ProductionOrderController : Controller
{

    readonly ILogger<ProductionOrderController> _logger;
    readonly IHttpClientService _httpClientService;
    readonly IProductionService _productionService;
    public ProductionOrderController(ILogger<ProductionOrderController> logger,
        IHttpClientService httpClientService, IProductionService productionService)
    {
        _logger = logger;
        _httpClientService = httpClientService;
        _productionService = productionService;
    }
    public IActionResult Index()
    {
        ViewData["Title"] = "Üretim Emirleri";
        return View();
    }

    [HttpPost]
    public async ValueTask<IActionResult> GetJsonResult()
    {
        return Json(new { data = GetProductions() });
    }

    private async IAsyncEnumerable<ProductionOrderListViewModel> GetProductions()
    {
        HttpClient httpClient = _httpClientService.GetOrCreateHttpClient();

        var result = _productionService.GetObjects(httpClient);

        await foreach (var item in result)
        {
            yield return new ProductionOrderListViewModel
            {
                ActualAmount = item.ActualAmount,
                ActualbeginDate = item.ActualbeginDate,
                ActualDuration = item.ActualDuration,
                ActualEndDate = item.ActualEndDate,
                BeginDate = item.BeginDate,
                Cancelled = item.Cancelled,
                Code = item.Code,
                ConversionFactor = item.ConversionFactor,
                Current = item.Current,
                Description = item.Description,
                DueDate = item.DueDate,
                EndDate = item.EndDate,
                FicheType = item.FicheType,
                OtherConversionFactor = item.OtherConversionFactor,
                PlanedBeginDate = item.PlanedBeginDate,
                PlanedDuration = item.PlanedDuration,
                PlanedEndDate = item.PlanedEndDate,
                PlannedAmount = item.PlannedAmount,
                Product = item.Product,
                ProductionDate = item.ProductionDate,
                ReferenceId = item.ReferenceId,
                StartDate = item.StartDate,
                Status = item.Status,
                StopDate = item.StopDate,
                SubUnitset = item.SubUnitset,
                Unitset = item.Unitset,
                RealizationRate = Convert.ToInt32((item.ActualAmount / item.PlannedAmount) * 100),



            };

        }



    }

}
