using MES.Client.Databases.SQLiteDatabase;
using MES.Client.ViewModels.LoginViewModels;
using MES.Client.Views.LoginViews;

namespace MES.Client;

public partial class App : Application
{
    IServiceProvider _serviceProvider;
    MESDatabase _database;
    public App(IServiceProvider serviceProvider, MESDatabase db)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _database = db;
        //MainPage = new AppShell();

        LoginViewModel viewModel = _serviceProvider.GetService<LoginViewModel>();
        MainPage = new LoginView(viewModel);
    }

	protected async override void OnStart()
	{
		base.OnStart();
        await _database.DeleteAllItemAsync();
	}
}

