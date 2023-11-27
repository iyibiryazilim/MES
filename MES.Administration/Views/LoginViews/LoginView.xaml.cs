using AndroidX.Lifecycle;
using MES.Administration.ViewModels.LoginViewModels;

namespace MES.Administration.Views.LoginViews;

public partial class LoginView : ContentPage
{
	LoginViewModel viewModel;
	public LoginView(LoginViewModel _viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel = _viewModel;


	}
}