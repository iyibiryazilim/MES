using LBS.Shared.Entity.Models;

namespace MES.ViewModels.SemiProductViewModels
{
    public class SemiProductListViewModel : SemiProduct
    {
        public short stockQuentity { get; set; } = default;

    }
}
