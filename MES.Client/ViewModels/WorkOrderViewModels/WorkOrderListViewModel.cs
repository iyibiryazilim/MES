using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Helpers.DeviceHelper;
using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.ViewModels.LoginViewModels;
using MES.Client.Views;
using MES.Client.Views.LoginViews;
using MES.Client.Views.PopupViews;
using MES.Client.Views.WorkOrderViews;
using MvvmHelpers;
using Shared.Entity.BaseModels;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MES.Client.ViewModels.WorkOrderViewModels;

public partial class WorkOrderListViewModel : BaseViewModel
{
    IHttpClientService _httpClientService;
    IWorkOrderService _workOrderService;
    //IBaseWorkOrderService _baseWorkOrderService;

	DeviceCommandHelper deviceCommandHelper = new DeviceCommandHelper(new HttpClient());

	public ObservableCollection<WorkOrder> Items { get; } = new();
    public ObservableCollection<WorkOrder> Results { get; } = new();
    public ObservableRangeCollection<dynamic> DisplayItems { get; } = new();

    public Command GetItemsCommand { get; }

    //[ObservableProperty]
    //ProductionWorkOrderList selectedItem;

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

    public WorkOrderListViewModel(IHttpClientService httpClientService, IWorkOrderService workOrderService)
    {
        Title = "İş Listesi";
        _httpClientService = httpClientService;
        _workOrderService = workOrderService;
        //_baseWorkOrderService = baseWorkOrderService;

		GetItemsCommand = new Command(async () => await GetItemsAsync());        
        //LoadMoreCommand = new Command(LoadMoreAsync);
    }


    //[RelayCommand]
    //async Task SetSelectedItemAsync(ProductionWorkOrderList item)
    //{
    //    if (IsBusy)
    //        return;

    //    try
    //    {
    //        IsBusy = true;
    //        IsRefreshing = true;

    //        if (item == null)
    //            return;

    //        foreach(var workOrder in Items)
    //        {
    //            workOrder.IsSelected = false;

    //        }
    //        Items.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;
    //        SelectedItem = item;

    //    } catch(Exception ex)
    //    {
    //        Debug.WriteLine(ex);
    //        await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
    //    } finally
    //    {
    //        IsBusy = false;
    //        IsRefreshing = false;
    //    }
    //}

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
			//DataResult<IEnumerable<BaseWorkOrder>> result = await _baseWorkOrderService.GetObjects(httpClient);
			var result = await _workOrderService.GetObjects(httpClient);

			if (result.IsSuccess)
            {
                if(result.Data.Any())
                {
                    foreach (var item in result.Data.Where(x => x.WorkstationCode == "E-02" ))
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
    async Task GoToDetailAsync(WorkOrder workOrder)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            IsRefreshing = true;
			deviceCommandHelper.SendCommandAsync("connectDevice", "http://192.168.1.18:32000").Wait();
			deviceCommandHelper.SendCommandAsync("initDevice", "http://192.168.1.18:32000").Wait();
			deviceCommandHelper.SendCommandAsync("startDevice", "http://192.168.1.18:32000").Wait();
			var popup = new StartWorkOrderPopupView(this);
			var result = await Shell.Current.ShowPopupAsync(popup);
			if (result is bool boolResult)
			{
				if (boolResult)
				{
					await Shell.Current.GoToAsync($"{nameof(WorkOrderDetailView)}", new Dictionary<string, object>
					{
						[nameof(WorkOrder)] = workOrder
					});
				}
				else
				{
					return;
				}
			}

			
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


    //[RelayCommand]
    //async Task OpenStartWorkOrderPopupAsync()
    //{
    //    var popup = new StartWorkOrderPopupView(this);
    //    var result = await Shell.Current.ShowPopupAsync(popup);
    //    if (result is bool boolResult)
    //    {
    //        if (boolResult)
    //        {

    //        }
    //        else
    //        {
    //            return;
    //        }
    //    }
    //}


	[RelayCommand]
    async Task OpenWorkOrderListModalAsync()
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

    //[RelayCommand]
    //async Task PerformSearchAsync(object text)
    //{
    //    if (IsBusy)
    //        return;

    //    try
    //    {
    //        IsBusy = true;
    //        if (!string.IsNullOrEmpty(text.ToString()))
    //        {
    //            Results.Clear();
    //            foreach (ProductionWorkOrderList item in Items.Where(x => x.MainProductCode.ToLower().Contains(text.ToString().ToLower())))
    //                Results.Add(item);
    //        }
    //        else
    //        {
    //            foreach (ProductionWorkOrderList item in Items)
    //                Results.Add(item);
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine(ex);
    //        await Application.Current.MainPage.DisplayAlert("Search Error :", ex.Message, "Tamam");
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //    }
    //}

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

