using LBS.Shared.Entity.Models;

namespace MES.ViewModels.OperationViewModels
{
    public class OperationListViewModel : Operation
    {
        public int ActiveWorkOrderCount { get; set; }
    }
}
