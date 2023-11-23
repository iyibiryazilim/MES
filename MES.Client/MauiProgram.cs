using CommunityToolkit.Maui;
using MES.Client.Databases.SQLiteDatabase;
using MES.Client.Helpers.DeviceHelper;
using MES.Client.Helpers.HttpClientHelpers;
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
using EmployeeDataStore = Shared.Middleware.DataStores.EmployeeDataStore;
using IEmployeeService = Shared.Middleware.Services.IEmployeeService;
using IWorkOrderService = Shared.Middleware.Services.IWorkOrderService;
using WorkOrderDataStore = Shared.Middleware.DataStores.WorkOrderDataStore;

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
		mauiAppBuilder.Services.AddSingleton<MESDatabase>();
		mauiAppBuilder.Services.AddSingleton<DeviceCommandHelper>();
		mauiAppBuilder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
		//mauiAppBuilder.Services.AddTransient<ICustomQueryService, CustomQueryDataStore>();
		mauiAppBuilder.Services.AddTransient<IStopCauseService, StopCauseDataStore>();
		mauiAppBuilder.Services.AddTransient<IEmployeeService, EmployeeDataStore>();
		mauiAppBuilder.Services.AddTransient<IWorkOrderService, WorkOrderDataStore>();

		return mauiAppBuilder;
	}

	//public static MauiAppBuilder RegisterAppDTO(this MauiAppBuilder mauiAppBuilder)
	//{
	//    mauiAppBuilder.Services.AddTransient<ICustomQueryDTO, CustomQueryDTO>();

	//    return mauiAppBuilder;
	//}

	public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddTransient<LoginViewModel>();
		mauiAppBuilder.Services.AddScoped<WorkOrderListViewModel>();
		mauiAppBuilder.Services.AddScoped<WorkOrderDetailViewModel>();
		mauiAppBuilder.Services.AddScoped<StopCauseListViewModel>();
		mauiAppBuilder.Services.AddTransient<WorkOrderListModalViewModel>();

		return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddTransient<LoginView>();
		mauiAppBuilder.Services.AddTransient<WorkOrderListView>();
		mauiAppBuilder.Services.AddScoped<WorkOrderDetailView>();
		mauiAppBuilder.Services.AddScoped<StopCauseListView>();
		mauiAppBuilder.Services.AddTransient<WorkOrderListModalView>();

		return mauiAppBuilder;
	}
}

