using MES.Administration.Helpers.HttpClientHelpers.HttpClientLBS;
using System;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace MES.Administration.ViewModels.PanelViewModels;

public partial class ProductionPanelViewModel : BaseViewModel
{
    IHttpClientLBSService _httpClientLBSService;
    IProductionOrderService _productionOrderService;

    public ObservableCollection<ProductionOrder> Items { get; } = new();

    public Command GetItemsCommand { get; }

    public ProductionPanelViewModel(IHttpClientLBSService httpClientLBSService, IProductionOrderService productionOrderService)
    {
        Title = "Üretim Genel Bakış";
        _httpClientLBSService = httpClientLBSService;   
        _productionOrderService = productionOrderService;


       GetItemsCommand = new Command(async () => await GetItemsAsync());
    }

    async Task GetItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if(Items.Count > 0)
                Items.Clear();

            var httpClient =_httpClientLBSService.GetOrCreateHttpClient();
            var result = await _productionOrderService.GetObjects(httpClient);
            if(result.IsSuccess)
            {
                if (result.Data.Any())
                {
                    foreach (var item in result.Data.Take(5))
                    {

                        await Task.Delay(100);
                        Items.Add(item);    
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Application.Current.MainPage.DisplayAlert("ProductionOrder Load Error :", ex.Message, "Tamam");
        }
        finally
        {
            IsBusy = false; 

        }



    }












}

