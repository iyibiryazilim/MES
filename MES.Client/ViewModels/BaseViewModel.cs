using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MES.Client.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    //[AlsoNotifyChangeFor(nameof(IsNotBusy))]
    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    string title;

    [ObservableProperty]
    bool isRefreshing;

    public bool IsNotBusy => !IsBusy;

    public BaseViewModel()
    {
    }
}

