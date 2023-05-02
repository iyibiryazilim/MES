using LBS.Shared.Entity.Models;

namespace MES.ViewModels.RawProductViewModel
{
    public class RawProductListViewModel : RawProduct
    {
        public short stockQuentity { get; set; } = default;
    }
}
