using CommunityToolkit.Mvvm.ComponentModel;

namespace MES.Client.ListModels;

public partial class WorkOrderList : ObservableObject
{
    [ObservableProperty]
    int referenceId;

    [ObservableProperty]
    int productionReferenceId;

    [ObservableProperty]
    int status;

    [ObservableProperty]
    string statusName;

    [ObservableProperty]
    string code;

    [ObservableProperty]
    int bOMasterReferenceId;

    [ObservableProperty]
    string bOMCode;

    [ObservableProperty]
    string bOMName;

    [ObservableProperty]
    int productReferenceId;

    [ObservableProperty]
    string productCode;

    [ObservableProperty]
    string productName;

    [ObservableProperty]
    DateTime planningStartDate;

    [ObservableProperty]
    TimeSpan planningStartTime;

    [ObservableProperty]
    DateTime planningEndDate;

    [ObservableProperty]
    TimeSpan planningEndTime;

    [ObservableProperty]
    double planningDuration;

    [ObservableProperty]
    double planningQuantity;

    [ObservableProperty]
    DateTime actualStartDate;

    [ObservableProperty]
    TimeSpan actualStartTime;

    [ObservableProperty]
    DateTime actualEndDate;

    [ObservableProperty]
    TimeSpan actualEndTime;

    [ObservableProperty]
    double actualDuration;

    [ObservableProperty]
    double actualQuantity;

    [ObservableProperty]
    int operationReferenceId;

    [ObservableProperty]
    string operationCode;

    [ObservableProperty]
    string operationName;

    [ObservableProperty]
    int workstationReferenceId;

    [ObservableProperty]
    string workstationCode;

    [ObservableProperty]
    string workstationName;

    [ObservableProperty]
    int routeReferenceId;

    [ObservableProperty]
    string routeCode;

    [ObservableProperty]
    string routeName;

    [ObservableProperty]
    double stopDuration;

    [ObservableProperty]
    bool isSelected;


    WorkOrderList()
    {
    }
}

