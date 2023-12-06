using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.ViewModels.WorkOrderViewModels;
using Shared.Entity.DTOs;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace MES.Client.ViewModels.StopCauseViewModels;

public partial class StopCauseListViewModel : BaseViewModel
{
	IHttpClientService _httpClientService;
	IStopCauseService _stopCauseService;
	IWorkOrderService _workOrderService;
	
	WorkOrderDetailViewModel workOrderDetailViewModel;
	public ObservableCollection<StopCause> StopCauseListItems { get; } = new();

	[ObservableProperty]
	StopCause selectedItem;

	public Command GetStopCauseListItemsCommand { get; }
	public Command InsertStopTransactionCommand { get; }
	public Command ChangeWorkOrderStatusToStoppedCommand { get; }	

	public StopCauseListViewModel(IHttpClientService httpClientService, IStopCauseService stopCauseService, WorkOrderDetailViewModel _workOrderDetailViewModel, IWorkOrderService workOrderService)
	{
		_httpClientService = httpClientService;
		_stopCauseService = stopCauseService;
		_workOrderService = workOrderService;
		workOrderDetailViewModel = _workOrderDetailViewModel;

		GetStopCauseListItemsCommand = new Command(async () => await GetStopCauseListItemsAsync());
		InsertStopTransactionCommand = new Command(async () => await InsertStopTransactionAsync());
		ChangeWorkOrderStatusToStoppedCommand = new Command(async () => await ChangeWorkOrderStatusToStoppedAsync());
	}
	
	async Task GetStopCauseListItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			IsRefreshing = true;

			if (StopCauseListItems.Count > 0)
				StopCauseListItems.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _stopCauseService.GetObjects(httpClient);

			if (result.IsSuccess)
			{
				if (result.Data.Any())
				{
					foreach (var item in result.Data)
					{
						await Task.Delay(250);
						StopCauseListItems.Add(item);
					}
				}
			}
		}

		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}
		finally
		{
			IsBusy = false;
			IsRefreshing = false;
		}
	}

	async Task InsertStopTransactionAsync()
	{
		//if (IsBusy)
		//	return;

		try
		{
			IsBusy = true;
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			StopTransactionForWorkOrderInsertDto stopTransactionForWorkOrderInsertDto = new()
			{
				WorkOrderReferenceId = workOrderDetailViewModel.WorkOrder.ReferenceId,
				StopCauseReferenceId = SelectedItem.ReferenceId,
				StopDate = DateTime.Now,
				StopTime = DateTime.Now.TimeOfDay
			};

			await _workOrderService.AddStopTransaction(httpClient, stopTransactionForWorkOrderInsertDto);
		} 
		catch(Exception ex)
		{
			Debug.WriteLine(ex);
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{
			IsBusy = false;
		}

	}

	async Task ChangeWorkOrderStatusToStoppedAsync()
	{
		//if (IsBusy)
		//	return;

		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _workOrderService.GetObjectById(httpClient, workOrderDetailViewModel.WorkOrder.ReferenceId);
			if (result.Data.Status != 2)
			{
				WorkOrderChangeStatusInsertDto workOrderChangeStatusInsertDto = new()
				{
					Status = 2,
					DeleteFiche = 0,
					FicheNo = result.Data.Code,
				};
				await _workOrderService.ChangeStatus(httpClient, workOrderChangeStatusInsertDto);
			}
			else
			{
				return;
			}

		}
		catch (Exception ex)
		{
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	async Task SetSelectedItemAsync(StopCause item)
	{
		//if (IsBusy)
		//	return;

		try
		{
			IsBusy = true;
			IsRefreshing = true;

			if (item == null)
				return;

			foreach (var stopCause in StopCauseListItems)
			{
				stopCause.IsSelected = false;
			}
			StopCauseListItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;
			SelectedItem = item;

		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{
			IsBusy = false;
			IsRefreshing = false;
		}
	}

	[RelayCommand]
	async Task StopButtonAsync()
	{
		//if(IsBusy) 
		//	return;

		if (SelectedItem is null)
			return;

		try
		{
			IsBusy = true;
			if (workOrderDetailViewModel is not null)
			{
				workOrderDetailViewModel?.Timer?.Stop();
				workOrderDetailViewModel?.LogoTimer?.Stop();
				InsertStopTransactionCommand.Execute(null);
				ChangeWorkOrderStatusToStoppedCommand.Execute(null);
			}
			await Shell.Current.GoToAsync("../..");
			SelectedItem = null;
		}
		catch(Exception ex)
		{
			Debug.WriteLine(ex);
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{

			IsBusy = false;
		}
	}

	[RelayCommand]
	async Task GoToBackAsync()
	{
		//if (IsBusy)
		//	return;
		try
		{
			IsBusy = true;
			await Shell.Current.GoToAsync("..");
		} catch(Exception ex)
		{
			Debug.WriteLine(ex);
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
}
