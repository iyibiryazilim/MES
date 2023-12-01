using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Administration.Helpers.HttpClientHelpers.HttpClientLBS;
using MES.Administration.Helpers.Mappers;
using MES.Administration.Helpers.Queries;
using MES.Administration.Models.ProductModels;
using MES.Administration.Models.WorkstationModels;
using Shared.Entity.Models;
using Shared.Middleware.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace MES.Administration.ViewModels.PanelViewModels;

public partial class WorkstationPanelViewModel : BaseViewModel
{
    IHttpClientLBSService _httpClientLBSService;
    IWorkstationService _workStationservice;
    ICustomQueryService _customQueryService;

    public ObservableCollection<WorkstationModel> Items { get; } = new();
   
    //Searchbar listesi
    public ObservableCollection<WorkstationModel> Results { get; } = new();

    [ObservableProperty]
    string searchText = string.Empty;
    public Command GetItemsCommand { get; }

    public WorkstationPanelViewModel(IHttpClientLBSService httpClientLBSService, IWorkstationService workStationservice, ICustomQueryService customQueryService)
    {
        Title = "İş İstasyonları Genel Bakış";
        _httpClientLBSService = httpClientLBSService;
        _workStationservice = workStationservice;
        _customQueryService = customQueryService;

        GetItemsCommand = new Command(async () => await GetItemsAsync());
    }

    async Task GetItemsAsync()
    {
        if (IsBusy)        
            return;
        

        try
        {
            IsBusy = true;

            if (Items.Count > 0)
                Items.Clear();

            var httpClient = _httpClientLBSService.GetOrCreateHttpClient();

            var result = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery());

            if (result.IsSuccess)
            {
                if (result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        await Task.Delay(100);

                        var obj = Mapping.Mapper.Map<WorkstationModel>(item);
                        Items.Add(obj);
                        Results.Add(obj);
                    }
                }
            }

        }
        catch (Exception ex) {

            Debug.WriteLine(ex.Message);
            await Application.Current.MainPage.DisplayAlert("Workstation Load Error :", ex.Message, "Tamam");
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
                foreach (WorkstationModel item in Items.Where(x => x.Code.ToLower().Contains(text.ToString().ToLower())))

                    Results.Add(item);
            }
            else
            {
                Results.Clear();

                foreach (WorkstationModel item in Items)

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

