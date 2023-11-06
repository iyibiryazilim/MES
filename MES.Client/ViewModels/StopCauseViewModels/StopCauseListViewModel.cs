using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Helpers.HttpClientHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YTT.Gateway.Middleware.Services;
using YTT.Gateway.Model.Models.ResultModels;
using YTT.Gateway.Model.Models.StopCauseModels;

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

    public ObservableCollection<StopCauseList> StopCauseListItems { get; } = new();

    [ObservableProperty]
    StopCauseList selectedItem;

   async Task GetStopCauseListItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            IsRefreshing = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            //var query = "SELECT * FROM StopCauseList";
            DataResult<IEnumerable<StopCauseList>> result = await _stopCauseService.GetObjectsAsync(httpClient);

            if(result.IsSuccess)
            {
                if(result.Data.Any())
                {
                    foreach(var item in result.Data)
                    {
                        StopCauseListItems.Add(item);
                    }
                }
            }
        }

        catch(Exception ex)
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
   async Task SetSelectedItemAsync(StopCauseList item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            IsRefreshing = true;

            if (item == null)
                return;

            foreach(var stopCause in StopCauseListItems)
            {
                stopCause.IsSelected = false;
            }
            StopCauseListItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;
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

    [RelayCommand]
    async Task StopButtonAsync()
    {
        await Shell.Current.GoToAsync("../..");
    }
}   
