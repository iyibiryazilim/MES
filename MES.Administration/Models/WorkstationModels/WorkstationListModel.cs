using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MES.Administration.Models.WorkstationModels;

public partial class WorkstationListModel : ObservableObject
{
    public WorkstationListModel()
    {
    }

    [ObservableProperty]
    int referenceId;

    [ObservableProperty]
    string code;

    [ObservableProperty]
    string name;
}

