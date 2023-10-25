using CommunityToolkit.Maui;
using LBS.WebAPI.Service.DataStores;
using LBS.WebAPI.Service.Services;
using MES.Client.DataStores;
using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.Services;
using MES.Client.ViewModels.LoginViewModels;
using MES.Client.ViewModels.WorkOrderViewModels;
using MES.Client.Views.LoginViews;
using MES.Client.Views.WorkOrderViews;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using The49.Maui.BottomSheet;

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
            .RegisterViews()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa-solid.otf", "FAS");
                fonts.AddFont("fa-regular.otf", "FAR");
                fonts.AddFont("fa-brands.otf", "FAB");
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
        mauiAppBuilder.Services.AddTransient<ICustomQueryService, CustomQueryDataStore>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterAppDTO(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddTransient<ICustomQueryDTO, CustomQueryDTO>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<LoginViewModel>();
        mauiAppBuilder.Services.AddSingleton<WorkOrderListViewModel>();
        mauiAppBuilder.Services.AddSingleton<WorkOrderDetailViewModel>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<WorkOrderListView>();
        mauiAppBuilder.Services.AddSingleton<WorkOrderDetailView>();
        mauiAppBuilder.Services.AddSingleton<LoginView>();

        return mauiAppBuilder;
    }
}

