using Android.Speech;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Administration.Helpers.HttpClientHelpers.HttpClientLBS;
using MES.Administration.Helpers.Mappers;
using MES.Administration.Helpers.Queries;
using MES.Administration.Models.ProductModels;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static Android.Content.ClipData;
namespace MES.Administration.ViewModels.PanelViewModels;

public partial class ProductPanelViewModel : BaseViewModel
{
    IHttpClientLBSService _httpClientLBSService;
    IEndProductService _endProduct;
    ICustomQueryService _customQueryService;
    public ObservableCollection<EndProductModel> Items { get; } = new();

    public ObservableCollection<EndProductModel> Results { get; } = new();

    [ObservableProperty]
    string searchText = string.Empty;
    public Command GetItemsCommand { get; }
    public ProductPanelViewModel(IHttpClientLBSService httpClientLBSService, IEndProductService endProduct,ICustomQueryService customQueryService)
    {
        Title = "Ürün Genel Bakış";
        _httpClientLBSService = httpClientLBSService;
        _endProduct = endProduct;
        _customQueryService = customQueryService;

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

            var result = await _customQueryService.GetObjects(httpClient, new ProductQuery().ProductListQuery());
            if (result.IsSuccess)
            {

                if (result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        await Task.Delay(100);
                        var obj = Mapping.Mapper.Map<EndProductModel>(item);
                        Items.Add(obj);
                        Results.Add(obj);
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

    [RelayCommand]
    async Task PerformSearchAsync(object text)
    {
        if (!IsBusy) return;
        try
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(text.ToString()))
            {
                Results.Clear();
                foreach (EndProductModel item in Items.Where(x => x.Code.ToLower().Contains(text.ToString().ToLower())))
                    Results.Add(item);
            }
            else
            {
                Results.Clear();
                foreach (EndProductModel item in Items)
                {
                    Results.Add(item);

                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Application.Current.MainPage.DisplayAlert("Search Error :", ex.Message, "Tamam");
        }
        finally
        {
            IsBusy = false;
        }

    }



}

