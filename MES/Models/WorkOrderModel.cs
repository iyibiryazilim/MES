using LBS.Shared.Entity.Models;

namespace MES.Models
{
    public class WorkOrderModel : WorkOrder
    {
        public int ActualAmount { get; set; } = default;
        public int PlannedAmount { get; set; } = default;
        public int RealizationRate { get; set; } = default;



    }
}
