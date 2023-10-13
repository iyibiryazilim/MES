using MES.Client.ViewModels.LoginViewModels;

namespace MES.Client.Views.LoginViews;

public partial class LoginView : ContentPage
{
    LoginViewModel _viewModel;
    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
