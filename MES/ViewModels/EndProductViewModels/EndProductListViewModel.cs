using LBS.Shared.Entity.Models;

namespace MES.ViewModels.ProductViewModels
{
    public class EndProductListViewModel : EndProduct
    {
        public double? stockQuentity { get; set; } = default;

        public EndProductListViewModel() { }
    }
}
