using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Helpers.HttpClientHelpers;

using MES.Client.Views.WorkOrderViews;

namespace MES.Client.ViewModels.LoginViewModels;

public partial class LoginViewModel : BaseViewModel
{
    public LoginViewModel()
    {
    }

    [ObservableProperty]
    string userCode;


    [RelayCommand]
    async Task GoToWorkOrderListAsync()
    {
        try
        {
            await Task.Delay(500);
            Application.Current.MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
        }
    }

    [RelayCommand]
    public async Task TextChangedAsync(string text)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            IsRefreshing = true;

            if(!string.IsNullOrEmpty(text))
            {
                UserCode = text;
                await Task.Delay(1000);
                Application.Current.MainPage = new AppShell();
            }
           

        } catch(Exception ex)
        {
            Debug.WriteLine(ex);
        } finally
        {
            IsBusy = false;
            IsRefreshing = false;
            UserCode = "";
        }        
    }
}

