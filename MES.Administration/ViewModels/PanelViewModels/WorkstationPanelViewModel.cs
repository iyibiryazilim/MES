
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kotlin.Properties;
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

   
    //Searchbar listesi
    public ObservableCollection<WorkstationModel> Results { get; } = new();

    [ObservableProperty]
    string searchText = string.Empty;

    [ObservableProperty]
    int currentIndex = 0;

    [ObservableProperty]
    int pageSize = 10;

    [ObservableProperty]
    string status = StatusTypes.WithoutStatus;

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

            if (Results.Count > 0)
                Results.Clear();

            CurrentIndex = Results.Count;
            var httpClient = _httpClientLBSService.GetOrCreateHttpClient();

            var result = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery(CurrentIndex,PageSize,SearchText,Status));

            if (result.IsSuccess)
            {
                if (result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        await Task.Delay(100);
                        var obj = Mapping.Mapper.Map<WorkstationModel>(item);
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
    async Task LoadMoreAsync()
    {
        if (IsBusy)
            return;


        try
        {
            IsBusy = true;

            if (Results.Count >= PageSize)
            {
                CurrentIndex=Results.Count;
                var httpClient = _httpClientLBSService.GetOrCreateHttpClient();

                var result = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery(CurrentIndex, PageSize,SearchText, Status));

                if (result.IsSuccess)
                {
                    if (result.Data.Any())
                    {
                        foreach (var item in result.Data)
                        {
                            await Task.Delay(100);
                            var obj = Mapping.Mapper.Map<WorkstationModel>(item);
                            Results.Add(obj);
                        }
                    }
                }
            }
         

        }
        catch (Exception ex)
        {

            Debug.WriteLine(ex.Message);
            await Application.Current.MainPage.DisplayAlert("Workstation Load Error :", ex.Message, "Tamam");
        }

        finally
        {
            IsBusy = false;

        }

    }

    [RelayCommand]
    public async Task PerformSearch(string text)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.Length >= 3)
                {
                    SearchText = text;
                    Results.Clear();
                    CurrentIndex = Results.Count;

                    var httpClient = _httpClientLBSService.GetOrCreateHttpClient();

                    var result = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery(CurrentIndex, PageSize, SearchText,Status));

                    if (result.IsSuccess)
                    {
                        if (result.Data.Any())
                        {
                            foreach (var item in result.Data)
                            {
                                await Task.Delay(100);
                                var obj = Mapping.Mapper.Map<WorkstationModel>(item);
                                Results.Add(obj);
                            }
                        }
                    }
                }

            }
            else
            {
                SearchText = string.Empty;
                Results.Clear();
                CurrentIndex = Results.Count;
                var httpClient = _httpClientLBSService.GetOrCreateHttpClient();

                var result = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery(CurrentIndex, PageSize, SearchText,Status));

                if (result.IsSuccess)
                {
                    if (result.Data.Any())
                    {
                        foreach (var item in result.Data)
                        {
                            await Task.Delay(100);
                            var obj = Mapping.Mapper.Map<WorkstationModel>(item);
                            Results.Add(obj);
                        }
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
                    CurrentIndex = Results.Count;
                    Status = StatusTypes.baslamadı;
                    var result = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery(CurrentIndex,PageSize,SearchText, Status));
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
                    CurrentIndex = Results.Count;
                    Status = StatusTypes.devam;

                    var result1 = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery(CurrentIndex, PageSize, SearchText, Status));
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
                    CurrentIndex = Results.Count;
                    Status = StatusTypes.durduruldu;

                    var result2 = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery(CurrentIndex, PageSize, SearchText, Status));
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
                    CurrentIndex = Results.Count;
                    Status = StatusTypes.tamamlandı;

                    var result3 = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery(CurrentIndex, PageSize, SearchText, Status));
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
                    CurrentIndex = Results.Count;
                    Status = StatusTypes.kapandı;

                    var result4 = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery(CurrentIndex, PageSize, SearchText, Status));
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

