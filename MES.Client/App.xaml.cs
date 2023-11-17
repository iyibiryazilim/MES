using MES.Client.Databases.SQLiteDatabase;
using MES.Client.ViewModels.LoginViewModels;
using MES.Client.Views.LoginViews;

namespace MES.Client;

public partial class App : Application
{
    IServiceProvider _serviceProvider;
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        //MainPage = new AppShell();

        LoginViewModel viewModel = _serviceProvider.GetService<LoginViewModel>();
        MainPage = new LoginView(viewModel);
    }
	protected override async void OnStart()
	{
		base.OnStart();
        MESDatabase mesDatabase = new MESDatabase();
        await mesDatabase.Init();
	}
}

