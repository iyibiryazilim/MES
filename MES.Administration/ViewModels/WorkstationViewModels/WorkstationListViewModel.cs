using LBS.WebAPI.Service.Services;
using MES.Administration.Helpers.HttpClientHelpers.HttpClientLBS;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System;
using System.Collections.ObjectModel;

namespace MES.Administration.ViewModels.WorkstationViewModels;

public partial class WorkstationListViewModel : BaseViewModel
{
    IHttpClientLBSService _httpClientLBSService;
    IWorkstationService _workStationservice;
    public WorkstationListViewModel(IHttpClientLBSService httpClientLBSService, IWorkstationService workStationservice)
    {
        Title = "İş İstasyonları";
        _httpClientLBSService = httpClientLBSService;
        _workStationservice = workStationservice;

        GetItemsCommand = new Command(async () => await GetItemsAsync());
    }

    ObservableCollection<Workstation> Result { get; } = new();

    public Command GetItemsCommand { get; }

    async Task GetItemsAsync()
    {
        if(IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            var httpClient = _httpClientLBSService.GetOrCreateHttpClient();
            var result= await _workStationservice.GetObjects(httpClient);
            foreach(var item in result.Data)
            {
                Result.Add(item);
            }

        }
        catch(Exception ex) { }
        
        finally
        {
            IsBusy = false;

        }

    }
}

