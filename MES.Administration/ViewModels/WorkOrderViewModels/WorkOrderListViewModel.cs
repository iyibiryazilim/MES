using MES.Administration.Helpers.HttpClientHelpers.HttpClientLBS;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System.Collections.ObjectModel;

namespace MES.Administration.ViewModels.WorkOrderViewModels;

public partial class WorkOrderListViewModel : BaseViewModel
{
    IHttpClientLBSService _httpClientLBSService;
    IWorkOrderService _workOrderService;
    public WorkOrderListViewModel( IHttpClientLBSService httpClientLBSService, IWorkOrderService workOrderService )
    {
        Title = "İş Emirleri";
        _httpClientLBSService = httpClientLBSService;
        _workOrderService = workOrderService;

        GetItemsCommand = new Command(async () => await GetItemsAsync());


    }

    public ObservableCollection<WorkOrder> Result { get; } = new();

    public Command GetItemsCommand { get; }


    async Task GetItemsAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            var httpClient = _httpClientLBSService.GetOrCreateHttpClient();
            var result = await _workOrderService.GetObjects(httpClient);
            if (result.IsSuccess)
            {
                if(result.Data.Any()) {
                    foreach (var item in result.Data)
                    {
                        Result.Add(item);
                    }
                }
            }
            

        }
        catch (Exception ex) { }

        finally
        {
            IsBusy = false;

        }

    }
}

