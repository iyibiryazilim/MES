using LBS.Shared.Entity.Models;

namespace MES.Models
{
    public class SemiProductModel : SemiProduct
    {
        public int StockQuentity { get; set; } = default;
        public DateTime LastTransactionDate { get; set; } = default;
        public int PurchaseQuentity { get; set; } = default;
        public int SellQuentity { get; set; } = default;
        /// <summary>
        /// Dönem başı stok miktarı
        /// </summary>
        public int FirstQuentity { get; set; } = default;

        /// <summary>
        /// Devir hızı
        /// </summary>
        public double RevolutionSpeed { get; set; } = default;
    }
}
