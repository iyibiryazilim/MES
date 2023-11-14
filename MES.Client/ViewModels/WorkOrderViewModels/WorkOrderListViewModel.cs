using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Helpers.DeviceHelper;
using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.Views;
using MES.Client.Views.LoginViews;
using MES.Client.Views.PopupViews;
using MES.Client.Views.WorkOrderViews;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using YTT.Gateway.Middleware.Services;
using YTT.Gateway.Model.Models.ResultModels;
using YTT.Gateway.Model.Models.WorkOrderModels;

namespace MES.Client.ViewModels.WorkOrderViewModels;

public partial class WorkOrderListViewModel : BaseViewModel
{
    IHttpClientService _httpClientService;
    IProductionWorkOrderService _productionWorkOrderService;

    DeviceCommandHelper deviceCommandHelper = new DeviceCommandHelper(new HttpClient());

    public ObservableCollection<ProductionWorkOrderList> Items { get; } = new();
    public ObservableCollection<ProductionWorkOrderList> Results { get; } = new();
    public ObservableRangeCollection<dynamic> DisplayItems { get; } = new();

    public Command GetItemsCommand { get; }

    [ObservableProperty]
    ProductionWorkOrderList selectedItem;

    //[ObservableProperty]
    //ObservableCollection<ProductionWorkOrderList> filterResult;

    [ObservableProperty]
    string searchText = string.Empty;

    // SearchBar genişliğini ekrana göre ayarlama fonksiyonu
    public double ScreenWidth
    {
        get
        {
            double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            double padding = 20; // Sağdan ve soldan çıkarmak istediğiniz padding miktarı

            double screenWidthWithPadding = screenWidth - (2 * padding) - padding;
            return screenWidthWithPadding;
        }
    }

    public WorkOrderListViewModel(IHttpClientService httpClientService, IProductionWorkOrderService productionWorkOrderService)
    {
        Title = "İş Listesi";
        _httpClientService = httpClientService;
        _productionWorkOrderService = productionWorkOrderService;

        GetItemsCommand = new Command(async () => await GetItemsAsync());        
        //LoadMoreCommand = new Command(LoadMoreAsync);
    }


    [RelayCommand]
    async Task SetSelectedItemAsync(ProductionWorkOrderList item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            IsRefreshing = true;

            if (item == null)
                return;

            foreach(var workOrder in Items)
            {
                workOrder.IsSelected = false;

            }
            Items.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;
            SelectedItem = item;

        } catch(Exception ex)
        {
            Debug.WriteLine(ex);
            await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
        } finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    async Task GetItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            IsRefreshing = true;

            if (Items.Count > 0)
                Items.Clear();
            if (Results.Count > 0)
                Results.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            DataResult<IEnumerable<ProductionWorkOrderList>> result  = await _productionWorkOrderService.GetObjectsAsync(httpClient);
            
            if(result.IsSuccess)
            {
                if(result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        Items.Add(item);
                        Results.Add(item);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Customer Error: ", $"{ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    async Task GoToDetailAsync(ProductionWorkOrderList productionWorkOrderList)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            IsRefreshing = true;
			//await deviceCommandHelper.SendCommandAsync("connectDevice", "http://192.168.1.25:32000");
			//await deviceCommandHelper.SendCommandAsync("initDevice", "http://192.168.1.25:32000");
			//await deviceCommandHelper.SendCommandAsync("startDevice", "http://192.168.1.25:32000");

			//await SetSelectedItemAsync(productionWorkOrderList);
			
		    await Shell.Current.GoToAsync($"{nameof(WorkOrderDetailView)}", new Dictionary<string, object>
		    {
			    [nameof(ProductionWorkOrderList)] = productionWorkOrderList
		    });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error :", ex.Message, "Tamam");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

	[RelayCommand]
    async Task OpenWorkOrderListModelAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(WorkOrderListModalView)}");
    }

    [RelayCommand]
    async Task LogoutHandlerAsync()
    {
        await Shell.Current.GoToAsync(nameof(LoginView));
    }

    //[RelayCommand]
    //async Task PerformSearchAsync(string text)
    //{
    //    if (IsBusy) return;
    //    try
    //    {
    //        IsBusy = true;

    //        SearchText = text.ToLower();
    //        if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
    //        {
    //            SearchText = "";
    //            FilterResult = Items;
    //        }  else
    //        {
    //            FilterResult = Items.Where(x => x.MainProductCode.ToLower().Contains(text.ToLower())).ToObservableCollection(); ;
    //            if (FilterResult.Any())
    //            {
    //                Items.Clear();
    //                foreach (var item in FilterResult)
    //                {
    //                    Items.Add(item);
    //                }
    //            }
    //        }
    //    }
    //    catch(Exception ex)
    //    {
    //        Debug.WriteLine(ex);
    //        await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
    //    } 
    //    finally
    //    {
    //        IsBusy = false;
    //    }

    //}

    [RelayCommand]
    async Task PerformSearchAsync(object text)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(text.ToString()))
            {
                Results.Clear();
                foreach (ProductionWorkOrderList item in Items.Where(x => x.MainProductCode.ToLower().Contains(text.ToString().ToLower())))
                    Results.Add(item);
            }
            else
            {
                foreach (ProductionWorkOrderList item in Items)
                    Results.Add(item);
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

    [RelayCommand]
    async Task ShowBottomSheetAsync()
    {
        var bottomSheetPage = new BottomSheetPage();
        bottomSheetPage.HasHandle = true;
        bottomSheetPage.HandleColor = Color.FromArgb("#009ef7");
        bottomSheetPage.HasBackdrop = true;

        await bottomSheetPage.ShowAsync(true);
    }
}

