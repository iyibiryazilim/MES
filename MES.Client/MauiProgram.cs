using CommunityToolkit.Maui;
using MES.Client.Databases.SQLiteDatabase;
using MES.Client.DataStores;
using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.Services;
using MES.Client.ViewModels.LoginViewModels;
using MES.Client.ViewModels.StopCauseViewModels;
using MES.Client.ViewModels.WorkOrderViewModels;
using MES.Client.Views.LoginViews;
using MES.Client.Views.StopCauseViews;
using MES.Client.Views.WorkOrderViews;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using Shared.Middleware.DataStores;
using Shared.Middleware.Services;
using The49.Maui.BottomSheet;
using YTT.Gateway.Middleware.DataStores;
using YTT.Gateway.Middleware.Services;
using EmployeeDataStore = Shared.Middleware.DataStores.EmployeeDataStore;
using IEmployeeService = Shared.Middleware.Services.IEmployeeService;
using IStopCauseService = YTT.Gateway.Middleware.Services.IStopCauseService;
using StopCauseDataStore = YTT.Gateway.Middleware.DataStores.StopCauseDataStore;

namespace MES.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseBottomSheet()
            .UseMicrocharts()
            .RegisterAppDataServices()
            .RegisterViewModels()
            .RegisterAppDTO()
            .RegisterAppDB()
            .RegisterViews()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa-solid.otf", "FAS");
                fonts.AddFont("fa-regular.otf", "FAR");
                fonts.AddFont("fa-brands.otf", "FAB");
            });
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    public static MauiAppBuilder RegisterAppDataServices(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<IHttpClientService, HttpClientService>();
        mauiAppBuilder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        //mauiAppBuilder.Services.AddTransient<ICustomQueryService, CustomQueryDataStore>();
        mauiAppBuilder.Services.AddTransient<IProductionWorkOrderService, ProductionWorkOrderDataStore>();
        mauiAppBuilder.Services.AddTransient<IStopCauseService, StopCauseDataStore>();
        mauiAppBuilder.Services.AddTransient<IEmployeeService, EmployeeDataStore>();
        mauiAppBuilder.Services.AddTransient<IWorkOrderService, WorkOrderDataStore>();


        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterAppDTO(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddTransient<ICustomQueryDTO, CustomQueryDTO>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterAppDB(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<MESDatabase>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<LoginViewModel>();
        mauiAppBuilder.Services.AddSingleton<WorkOrderListViewModel>();
        mauiAppBuilder.Services.AddSingleton<WorkOrderDetailViewModel>();
        mauiAppBuilder.Services.AddSingleton<StopCauseListViewModel>();
        mauiAppBuilder.Services.AddSingleton<WorkOrderListModalViewModel>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<WorkOrderListView>();
        mauiAppBuilder.Services.AddSingleton<WorkOrderDetailView>();
        mauiAppBuilder.Services.AddSingleton<LoginView>();
        mauiAppBuilder.Services.AddSingleton<StopCauseListView>();
        mauiAppBuilder.Services.AddSingleton<WorkOrderListModalView>();

        return mauiAppBuilder;
    }
}

