using LBS.Shared.Entity.Models;
namespace MES.ViewModels.WorkOrderViewModels
{
    public class WorkOrderListViewModel : WorkOrder
	{
		public int RealizationRate { get; set; } = default;
        public double ActualAmount { get; set; } = default;
        public double PlannedAmount { get; set; } = default;


        public WorkOrderListViewModel()
		{
		}
	}
}

