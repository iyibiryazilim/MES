
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
using System.Security.Cryptography.X509Certificates;
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

          await  LoadingAsync();

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
    //Arama
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
                    await LoadingAsync();

                }

            }
            else
            {
                SearchText = string.Empty;
                await LoadingAsync();


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

    //filtreleme
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

            //var httpClient = _httpClientLBSService.GetOrCreateHttpClient();

            switch (response)
            {
                case "Başlamadı":
                    Status = StatusTypes.NotStarted;
                    await LoadingAsync();
                    if (Results.Any())
                    {
                        hasResults = true;
                    }

                        break;
                case "Devam Ediyor":
                    Status = StatusTypes.Continues;
                    await LoadingAsync();
                    if (Results.Any())
                    {
                        hasResults = true;
                    }
                    break;
                case "Durduruldu":
                    Status = StatusTypes.Stopped;
                    await LoadingAsync();
                    if (Results.Any())
                    {
                        hasResults = true;
                    }
                    break;
                case "Tamamlandı":
                    Status = StatusTypes.Completed;
                    await LoadingAsync();
                    if (Results.Any())
                    {
                        hasResults = true;
                    }
                    break;
                case "Kapandı":
                    Status = StatusTypes.Closed;
                    if (Results.Any())
                    {
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

    //genel fonksiyon
    public async Task LoadingAsync()
    {
        Results.Clear();

        CurrentIndex = Results.Count;
        var httpClient = _httpClientLBSService.GetOrCreateHttpClient();

        var result = await _customQueryService.GetObjects(httpClient, new WorkstationQuery().WorkstationListQuery(CurrentIndex, PageSize, SearchText, Status));

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

