using LBS.Shared.Entity.Models;

namespace MES.ViewModels.SemiProductViewModels
{
    public class SemiProductDetailViewModel : SemiProduct
    {
        public short stockQuentity { get; set; } = default;

    }
}
