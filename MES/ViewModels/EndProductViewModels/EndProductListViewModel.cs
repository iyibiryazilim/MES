using LBS.Shared.Entity.Models;
using MES.Models;

namespace MES.ViewModels.ProductViewModels
{
    public class EndProductListViewModel : EndProduct
    {
		public EndProductModel? EndProductModel { get; set; }

		public EndProductListViewModel() { }
    }
}
