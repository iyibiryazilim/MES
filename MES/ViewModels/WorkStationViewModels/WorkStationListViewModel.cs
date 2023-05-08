using LBS.Shared.Entity.Models;

namespace MES.ViewModels.WorkStationViewModels
{
    public class WorkStationListViewModel 
    {
		public int Count { get; set; } = default;
		public int CompletedCount { get; set; } = default;
		public int WaitingCount { get; set; } = default;
		public int StopCount { get; set; } = default;
		public int CanceledCount { get; set; } = default;


	}
}
