using LBS.Shared.Entity.Models;

namespace MES.ViewModels.OperationOrderViewModels
{
	public class ProductionOrderListViewModel : ProductionOrder
	{
        public int RealizationRate { get; set; } = default;

        
    }
}
