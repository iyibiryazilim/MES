using CommunityToolkit.Maui;
using MES.Administration.Helpers.HttpClientHelpers.HttpClientLBS;
using MES.Administration.ViewModels.BOMViewModels;
using MES.Administration.ViewModels.LoginViewModels;
using MES.Administration.ViewModels.PanelViewModels;
using MES.Administration.ViewModels.ProductionOrderViewModels;
using MES.Administration.ViewModels.ProductViewModels;
using MES.Administration.ViewModels.WorkOrderViewModels;
using MES.Administration.Views.BOMViews;
using MES.Administration.Views.LoginViews;
using MES.Administration.Views.PanelViews;
using MES.Administration.Views.ProductionOrderViews;
using MES.Administration.Views.ProductViews;
using MES.Administration.Views.WorkOrderViews;
using MES.Administration.Views.WorkstationViews;
using Microsoft.Extensions.Logging;
using Shared.Middleware.DataStores;
using Shared.Middleware.Services;
using SimpleToolkit.Core;
using SimpleToolkit.SimpleShell;

namespace MES.Administration;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .RegisterAppDataServices()
            .RegisterViewModels()
            .RegisterViews()
            .UseSimpleToolkit()
            .UseSimpleShell()
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

#if ANDROID || IOS
        builder.DisplayContentBehindBars();
#endif

        return builder.Build();
    }

    public static MauiAppBuilder RegisterAppDataServices(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<IHttpClientLBSService, HttpClientLBSService>();
        mauiAppBuilder.Services.AddSingleton<IConnectivity>(Connectivity.Current);        
        mauiAppBuilder.Services.AddTransient<IWorkstationService, WorkstationDataStore>();
        mauiAppBuilder.Services.AddTransient<IWorkOrderService, WorkOrderDataStore>();
        mauiAppBuilder.Services.AddTransient<IEndProductService, EndProductDataStore>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<LoginViewModel>();
        mauiAppBuilder.Services.AddSingleton<PanelViewModel>();
        mauiAppBuilder.Services.AddSingleton<ProductionPanelViewModel>();
        mauiAppBuilder.Services.AddSingleton<WorkstationPanelViewModel>();
        mauiAppBuilder.Services.AddSingleton<ProductPanelViewModel>();
        mauiAppBuilder.Services.AddSingleton<MaintenancePanelViewModel>();

        mauiAppBuilder.Services.AddSingleton<BOMListViewModel>();
        mauiAppBuilder.Services.AddSingleton<ProductionOrderListViewModel>();
        mauiAppBuilder.Services.AddSingleton<ProductListViewModel>();
        mauiAppBuilder.Services.AddSingleton<WorkOrderListViewModel>();
        
    


        return mauiAppBuilder;
    }


    public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<LoginView>();
        mauiAppBuilder.Services.AddSingleton<PanelView>();
        mauiAppBuilder.Services.AddSingleton<ProductionPanelView>();
        mauiAppBuilder.Services.AddSingleton<WorkstationPanelView>();
        mauiAppBuilder.Services.AddSingleton<ProductPanelView>();
        mauiAppBuilder.Services.AddSingleton<MaintenancePanelView>();

        mauiAppBuilder.Services.AddSingleton<BOMListView>();
        mauiAppBuilder.Services.AddSingleton<ProductionOrderListView>();
        mauiAppBuilder.Services.AddSingleton<ProductListView>();
        mauiAppBuilder.Services.AddSingleton<WorkOrderListView>();
        mauiAppBuilder.Services.AddSingleton<WorkstationListView>();
        

        return mauiAppBuilder;
    }
}

