using CommunityToolkit.Maui.Core.Platform;
using MES.Client.ViewModels.LoginViewModels;

namespace MES.Client.Views.LoginViews;

public partial class LoginView : ContentPage
{
    LoginViewModel _viewModel;
    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        Loaded += LoginPageLoaded;
        BindingContext = _viewModel = viewModel;
    }

    private void LoginPageLoaded(object sender, EventArgs e)
    {
        Task.Delay(750).ContinueWith(x => txtUsername.Focus()); 
    }
}
