using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Helpers;
using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.Views;
using MES.Client.Views.LoginViews;
using MES.Client.Views.PopupViews;
using MES.Client.Views.WorkOrderViews;
using MvvmHelpers;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MES.Client.ViewModels.WorkOrderViewModels;

public partial class WorkOrderListViewModel : BaseViewModel
{
	IHttpClientService _httpClientService;
	IWorkOrderService _workOrderService;

	[ObservableProperty]
	public string currentEmployee;

	[ObservableProperty]
	string searchText = string.Empty;

	public ObservableCollection<WorkOrder> Items { get; } = new();
	public ObservableCollection<WorkOrder> Results { get; } = new();

	public Command GetItemsCommand { get; }
	public Command GetCurrentEmployeeCommand { get; }
	public Command LogoutCommand { get; }

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

		GetItemsCommand = new Command(async () => await GetItemsAsync());
		GetCurrentEmployeeCommand = new Command(async () => await GetCurrentUserAsync());
		LogoutCommand = new Command(async () => await LogoutHandlerAsync());

		GetCurrentEmployeeCommand.Execute(null);
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
			int[] status = new int[] { 0, 2, 1, 3, 4 };
			var result = await _workOrderService.GetByStatus(httpClient, status);
			if (result.IsSuccess)
			{
				if (result.Data.Any())
				{
					foreach (var item in result.Data)
					{
						await Task.Delay(250);
						Items.Add(item);
						Results.Add(item);
					}
				}
			}

		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			await Shell.Current.DisplayAlert(" Error: ", $"{ex.Message}", "Tamam");
		}
		finally
		{
			IsBusy = false;
			IsRefreshing = false;
		}
	}

	public async Task GetCurrentUserAsync()
	{
		if (IsBusy) return;
		try
		{
			IsBusy = true;
			IsRefreshing = true;

			var result = await new CurrentUserHelper().GetCurrentEmployeeAsync();
			if (result is not null)
			{
				CurrentEmployee = result ?? string.Empty;
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

	[RelayCommand]
	async Task GoToDetailAsync(WorkOrder workOrder)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			IsRefreshing = true;
			
			var popup = new StartWorkOrderPopupView(this);
			var result = await Shell.Current.ShowPopupAsync(popup);
			if (result is bool boolResult)
			{
				if (boolResult)
				{
					await Task.Delay(300);
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


	[RelayCommand]
	async Task OpenWorkOrderListModalAsync()
	{
		await Shell.Current.GoToAsync($"{nameof(WorkOrderListModalView)}");
	}

	async Task LogoutHandlerAsync()
	{
		if(IsBusy)
		{
			return;
		}
		try
		{
			IsBusy = true;

			SecureStorage.Remove("CurrentUserName");
			await Shell.Current.GoToAsync(nameof(LoginView));
		} catch(Exception ex)
		{
			Debug.WriteLine(ex.Message);
			await Application.Current.MainPage.DisplayAlert("Log out error :", "Çıkış yapılırken bir hata oluştu", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
		
	}

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
				foreach (WorkOrder item in Items.Where(x => x.ProductCode.ToLower().Contains(text.ToString().ToLower())))
					Results.Add(item);
			}
			else
			{
				Results.Clear();
				foreach (WorkOrder item in Items)
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

