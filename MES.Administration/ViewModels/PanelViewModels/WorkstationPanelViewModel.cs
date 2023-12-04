
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
using System.Net.Http;
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
            if (Results.Count > 0)
                Results.Clear();


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

    [RelayCommand]
    public async Task ShowFilterAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            string response = await Shell.Current.DisplayActionSheet("Filtrele", "Vazgeç", null, "Başlamadı", "Devam Ediyor", "Durduruldu", "Tamamlandı", "Kapandı");
            bool hasResults = false;

            var httpClient = _httpClientLBSService.GetOrCreateHttpClient();

            switch (response)
            {
                case "Başlamadı":
                    Results.Clear();
                    var result = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationFilterQuery(0));
                    foreach (var item in result.Data)
                    {
                        await Task.Delay(500);
                        var obj = Mapping.Mapper.Map<WorkstationModel>(item);
                        Results.Add(obj);
                        hasResults = true;
                    }
                    break;
                case "Devam Ediyor":
                    Results.Clear();
                    var result1 = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationFilterQuery(1));
                    foreach (var item in result1.Data)
                    {
                        await Task.Delay(500);
                        var obj = Mapping.Mapper.Map<WorkstationModel>(item);
                        Results.Add(obj);
                        hasResults = true;
                    }
                    break;
                case "Durduruldu":
                    Results.Clear();
                    var result2 = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationFilterQuery(2));
                    foreach (var item in result2.Data)
                    {
                        await Task.Delay(500);
                        var obj = Mapping.Mapper.Map<WorkstationModel>(item);
                        Results.Add(obj);
                        hasResults = true;
                    }
                    break;
                case "Tamamlandı":
                    Results.Clear();
                    var result3 = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationFilterQuery(3));
                    foreach (var item in result3.Data)
                    {
                        await Task.Delay(500);
                        var obj = Mapping.Mapper.Map<WorkstationModel>(item);
                        Results.Add(obj);
                        hasResults = true;
                    }
                    break;
                case "Kapandı":
                    Results.Clear();
                    var result4 = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationFilterQuery(4));
                    foreach (var item in result4.Data)
                    {
                        await Task.Delay(500);
                        var obj = Mapping.Mapper.Map<WorkstationModel>(item);
                        Results.Add(obj);

                        hasResults = true;
                    }
                    break;
                default:
                    //geçerli durum yoksa
                    hasResults = false;
                    break;
            }
            if (!hasResults)
            {
                await Application.Current.MainPage.DisplayAlert("Bilgi", "Seçilen duruma uygun veri bulunamadı.", "Tamam");
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Application.Current.MainPage.DisplayAlert("Filter Error :", ex.Message, "Tamam");

        }
        finally
        {
            IsBusy = false;
        }
    }

}

