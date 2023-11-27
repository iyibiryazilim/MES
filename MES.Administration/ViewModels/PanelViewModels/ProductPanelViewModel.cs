using MES.Administration.Helpers.HttpClientHelpers.HttpClientLBS;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace MES.Administration.ViewModels.PanelViewModels;

public partial class ProductPanelViewModel : BaseViewModel
{
    IHttpClientLBSService _httpClientLBSService;
    IEndProductService _endProduct;
    public ObservableCollection<EndProduct> Items { get; } = new();

    public Command GetItemsCommand { get; }
    public ProductPanelViewModel(IHttpClientLBSService httpClientLBSService, IEndProductService endProduct)
    {
        Title = "Ürün Genel Bakış";
        _httpClientLBSService = httpClientLBSService;
        _endProduct = endProduct;

        GetItemsCommand = new Command(async () => await GetItemsAsync());
    }

 


    async Task GetItemsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            if (Items.Count > 0)
                Items.Clear();

            var httpClient = _httpClientLBSService.GetOrCreateHttpClient();

            var result = await _endProduct.GetObjects(httpClient);
            if (result.IsSuccess)
            {

                if (result.Data.Any())
                {
                    foreach (var item in result.Data)
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
            await Application.Current.MainPage.DisplayAlert("Product Load Error :", ex.Message, "Tamam");

        }
        finally
        {
            IsBusy = false;
        }
    }

}

