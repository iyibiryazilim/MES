﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Helpers.HttpClientHelpers;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace MES.Client.ViewModels.StopCauseViewModels;

public partial class StopCauseListViewModel : BaseViewModel
{
	IHttpClientService _httpClientService;
	IStopCauseService _stopCauseService;

	public StopCauseListViewModel(IHttpClientService httpClientService, IStopCauseService stopCauseService)
	{
		_httpClientService = httpClientService;
		_stopCauseService = stopCauseService;

		GetStopCauseListItemsCommand = new Command(async () => await GetStopCauseListItemsAsync());
	}
	public Command GetStopCauseListItemsCommand { get; }

	public ObservableCollection<StopCause> StopCauseListItems { get; } = new();

	[ObservableProperty]
	StopCause selectedItem;

	async Task GetStopCauseListItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			IsRefreshing = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _stopCauseService.GetObjects(httpClient);

			if (result.IsSuccess)
			{
				if (result.Data.Any())
				{
					foreach (var item in result.Data)
					{
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

	[RelayCommand]
	async Task SetSelectedItemAsync(StopCause item)
	{
		if (IsBusy)
			return;

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
		await Shell.Current.GoToAsync("../..");
	}

	[RelayCommand]
	async Task GoToBackAsync()
	{
		await Shell.Current.GoToAsync("..");
	}
}
