using Shared.Entity.Models;

namespace MES.Administration.Models.WorkstationModels;

public class WorkstationModel : Workstation
{


    DateTime? maintenanceOn;
    public DateTime? MaintenanceOn
    {
        get => maintenanceOn;
        set
        {

            maintenanceOn = value;
            NotifyPropertyChanged();
        }
    }

    TimeSpan? maintenanceRemainingDay;
    public TimeSpan? MaintenanceRemainingDay
    {
        get => maintenanceRemainingDay;
        set
        {

            maintenanceRemainingDay = value;
            NotifyPropertyChanged();
        }
    }


    short? status;
    public short? Status
    {
        get => status;
        set
        {
            status = value;
            NotifyPropertyChanged();
        }
    }
    string statusName;
    public string StatusName
    {
        get => statusName;
        set
        {
            statusName = value;
            NotifyPropertyChanged();
        }
    }

    double? oEE;
    public double? OEE
    {
        get => oEE;
        set
        {

            oEE = value;
            NotifyPropertyChanged();
        }
    }
    double? quality;
    public double? Quality
    {
        get => quality;
        set
        {

            quality = value;
            NotifyPropertyChanged();
        }
    }

    double? performance;
    public double? Performance
    {
        get => performance;
        set
        {

            performance = value;
            NotifyPropertyChanged();
        }
    }

    double? probability;
    public double? Probability
    {
        get => probability;
        set
        {

            probability = value;
            NotifyPropertyChanged();
        }
    }
    int? workOrderReferenceId;
    public int? WorkOrderReferenceId
    {
        get => workOrderReferenceId;
        set
        {
            if (value != workOrderReferenceId)
            {
                workOrderReferenceId = value;
                NotifyPropertyChanged(nameof(WorkOrderReferenceId));
            }
        }
    }
}
