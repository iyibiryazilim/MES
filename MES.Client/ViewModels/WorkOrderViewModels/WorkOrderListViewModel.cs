using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LBS.Shared.Entity.Models;
using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.Helpers.Mappers;
using MES.Client.ListModels;
using MES.Client.Services;
using MES.Client.Views;
using MES.Client.Views.WorkOrderViews;
using MvvmHelpers;

namespace MES.Client.ViewModels.WorkOrderViewModels;

public partial class WorkOrderListViewModel : BaseViewModel
{
    IHttpClientService _httpClientService;
    ICustomQueryDTO _customQueryDTO;

    public ObservableCollection<WorkOrderList> Items { get; } = new();
    public ObservableRangeCollection<dynamic> DisplayItems { get; } = new();

    public Command GetItemsCommand { get; }
    public Command LoadMoreCommand { get; }

    static int pageSize = 20;
    static int currentIndex = 0;

    [ObservableProperty]
    WorkOrderList selectedItem;


    private bool isSelectedItem;

    public bool IsSelectedItem
    {
        get { return SelectedItem != null ? true : false; }
        set
        {
            IsSelectedItem = value;
            OnPropertyChanged(nameof(IsSelectedItem));
        }
    }


    public bool ButtonStatus => SelectedItem == null ? false : true;

    string query = @$"SELECT 
[ReferenceId] = LGMAIN.LOGICALREF,
[ProductionReferenceId] = LGMAIN.PRODORDREF,
[Status] = LGMAIN.LINESTATUS,
[StatusName] = '',
[Code] = LGMAIN.LINENO_,
[BOMasterReferenceId] = BOMASTER.LOGICALREF,
[BOMCode] = BOMASTER.CODE,
[BOMName] = BOMASTER.NAME,
[ProductReferenceId] = ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[PlanningStartDate] = LGMAIN.OPBEGDATE,
[PlanningStartTime] = LGMAIN.OPBEGTIME,
[PlanningEndDate] = LGMAIN.OPDUEDATE,
[PlanningEndTime] = LGMAIN.OPDUETIME,
[PlanningDuration] = LGMAIN.PLNDURATION,
[PlanningQuantity] = 0,
[ActualStartDate] = LGMAIN.ACTBEGDATE,
[ActualStartTime] = LGMAIN.ACTBEGTIME,
[ActualEndDate] = LGMAIN.ACTDUEDATE,
[ActualEndTime] = LGMAIN.ACTDUETIME,
[ActualDuration] = LGMAIN.ACTDURATION,
[OperationReferenceId] = OPERTION.LOGICALREF,
[OperationCode] = OPERTION.CODE,
[OperationName] = OPERTION.NAME,
[WorkstationReferenceId] = WORKSTAT.LOGICALREF,
[WorkstationCode] = WORKSTAT.CODE,
[WorkstationName] = WORKSTAT.NAME,
[RouteReferenceId] =0,
[RouteCode] = '',
[RouteName] = '',
[StopDuration] = LGMAIN.STPDURATION


FROM LG_003_DISPLINE AS LGMAIN
LEFT JOIN LG_003_PRODORD AS PRODORD ON LGMAIN.PRODORDREF = PRODORD.LOGICALREF
LEFT JOIN LG_003_BOMASTER AS BOMASTER ON LGMAIN.BOMMASTERREF = BOMASTER.LOGICALREF AND LGMAIN.REVREF = BOMASTER.VALIDREVREF
LEFT JOIN LG_003_ITEMS AS ITEMS ON LGMAIN.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_003_OPERTION AS OPERTION ON LGMAIN.OPERATIONREF = OPERTION.LOGICALREF
LEFT JOIN LG_003_WORKSTAT AS WORKSTAT ON LGMAIN.WSREF = WORKSTAT.LOGICALREF
ORDER BY LGMAIN.LOGICALREF ASC
OFFSET {currentIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY
";

    public WorkOrderListViewModel(IHttpClientService httpClientService, ICustomQueryDTO customQueryDTO)
    {
        Title = "İş Listesi";
        _httpClientService = httpClientService;
        _customQueryDTO = customQueryDTO;

        GetItemsCommand = new Command(async () => await GetItemsAsync());
        LoadMoreCommand = new Command(LoadMoreAsync);
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




            await foreach (var item in _customQueryDTO.GetObjectsAsync(_httpClientService, query))
            {
                await Task.Delay(100);
                var obj = Mapping.Mapper.Map<WorkOrderList>(item);
                Items.Add(obj);
            }

            //DisplayItems.AddRange(Items);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Customer Error: ", $"{ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    async void LoadMoreAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            currentIndex = Items.Count;

            await foreach (var item in _customQueryDTO.GetObjectsAsync(_httpClientService, query))
            {
                await Task.Delay(100);
                var obj = Mapping.Mapper.Map<WorkOrderList>(item);
                Items.Add(obj);
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
        }
        finally
        {
            IsBusy = false;

        }
    }

    [RelayCommand]
    async Task GoToDetailAsync(WorkOrderList workOrderList)
    {
        try
        {
            await Shell.Current.GoToAsync($"{nameof(WorkOrderDetailView)}", new Dictionary<string, object>
            {
                ["WorkOrderList"] = workOrderList
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error :", ex.Message, "Tamam");
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

    [RelayCommand]
    async Task GetSelectedItemAsync(WorkOrderList item)
    {
        if (IsBusy)
            return;

        try
        {

            IsBusy = true;

            foreach (var workOrder in Items)
                workOrder.IsSelected = false;

            Items.First(x => x.Code == item.Code).IsSelected = true;
            SelectedItem = Items.First(x => x.Code == item.Code);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Shell.Current.DisplayAlert("Error : ", "SelectedItem False", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }

        await Task.FromResult(() => SelectedItem = item);
    }
}

