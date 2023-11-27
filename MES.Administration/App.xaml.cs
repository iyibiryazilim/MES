using MES.Administration.ViewModels.LoginViewModels;
using MES.Administration.Views.LoginViews;

namespace MES.Administration;

public partial class App : Application
{
    IServiceProvider _serviceProvider;
    public App(IServiceProvider serviceProvider)
	{

       

        InitializeComponent();
        _serviceProvider = serviceProvider;
       //LoginViewModel viewModel = _serviceProvider.GetService<LoginViewModel>();
       //MainPage = new LoginView(viewModel);

      MainPage = new AppShell();
	}
}

