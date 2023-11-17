using MES.Administration.Helpers.HttpClientHelpers.HttpClientLBS;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System;
using System.Collections.ObjectModel;

namespace MES.Administration.ViewModels.ProductViewModels;

public partial class ProductListViewModel : BaseViewModel
{
    IHttpClientLBSService _httpClientLBSService;
    IEndProductService _endProduct;
    public ProductListViewModel(IHttpClientLBSService httpClientLBSService , IEndProductService endProduct)
    {
        Title = "Ürünler";
        _httpClientLBSService = httpClientLBSService;
        _endProduct = endProduct;

        GetItemsCommand = new Command(async () => await GetItemsAsync());
    }

    public ObservableCollection<EndProduct> Result { get; } = new();

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
            var result= await _endProduct.GetObjects(httpClient);
           if (result.IsSuccess)
            {
                if(result.Data.Any()) {
                    foreach (var item in result.Data)
                    {
                        await Task.Delay(10);
                        Result.Add(item);
                    }
                }
            }


        }
        catch(Exception ex) 
        {
            throw ex;

        }
        finally
        {
            IsBusy = false;
        }
    }

}

