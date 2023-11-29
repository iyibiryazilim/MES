using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.ViewModels.WorkOrderViewModels;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MES.Client.ViewModels.StopTransactionViewModels;

public partial class StopTransactionListViewModel : BaseViewModel
{
	IHttpClientService _httpClientService;
	IStopTransactionService _stopTransactionService;
	WorkOrderDetailViewModel workOrderDetailViewModel;
	public ObservableCollection<StopTransaction> StopTransactionListItems { get; } = new();

	public Command GetItemsCommand { get; }
	public Command GoToBackCommand { get; }

	public StopTransactionListViewModel(IHttpClientService httpClientService, IStopTransactionService stopTransactionService, WorkOrderDetailViewModel _workOrderDetailViewModel)
	{
		Title = "Duruş Hareketleri";
		_httpClientService = httpClientService;
		_stopTransactionService = stopTransactionService;
		workOrderDetailViewModel = _workOrderDetailViewModel;

		GetItemsCommand = new Command(async () => await GetItemsAsync());
		GoToBackCommand = new Command(async () => await GoToBackAsync());
	}

	async Task GetItemsAsync()
	{
		if(IsBusy)
			return;

		try
		{
			IsBusy = true;

			if(StopTransactionListItems.Count > 0)
				StopTransactionListItems.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _stopTransactionService.GetObjects(httpClient);
			//var result = await _stopTransactionService.GetObjectById(httpClient, workOrderDetailViewModel.WorkOrder.ReferenceId);

			if (result.IsSuccess)
			{
				if (result.Data.Any())
				{
					foreach (var item in result.Data)
					{
						await Task.Delay(250);
						StopTransactionListItems.Add(item);
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
		}
	}

	async Task GoToBackAsync()
	{
		if(IsBusy)
			return;

		try
		{
			IsBusy = true;	
			await Shell.Current.GoToAsync("..");
		}
		catch(Exception ex)
		{
			Debug.WriteLine(ex);
		}
		finally
		{
			IsBusy = false;
		}
		
	}
}
