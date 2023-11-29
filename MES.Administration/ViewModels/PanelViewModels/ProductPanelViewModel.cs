using Android.Speech;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public ObservableCollection<EndProduct> Results { get; } = new();

    [ObservableProperty]
    string searchText = string.Empty;
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
                        Results.Add(item);
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
        if(!IsBusy) return;
        try
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(text.ToString()))
            {
                Results.Clear();
                foreach (EndProduct item in Items.Where(x=> x.Code.ToLower().Contains(text.ToString().ToLower())))
                   Results.Add(item);   
            }
            else
            {
                Results.Clear();
                foreach (EndProduct item in Items)
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

