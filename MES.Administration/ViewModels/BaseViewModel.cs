using CommunityToolkit.Mvvm.ComponentModel;

namespace MES.Administration.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
    {
    }

    [ObservableProperty]
    string title;

    [ObservableProperty]
    bool isBusy;


    public bool IsNotBusy => !IsBusy;
}

