using LBS.Shared.Entity.Models;

namespace MES.Models
{
    public class SemiProductModel : SemiProduct
    {
        public int StockQuentity { get; set; } = default;
        public DateTime LastTransactionDate { get; set; } = default;
        /// <summary>
        /// Alım Miktarı
        /// </summary>
        public int PurchaseQuentity { get; set; } = default;
        /// <summary>
        /// Satış Miktarı
        /// </summary>
        public int SellQuentity { get; set; } = default;
        /// <summary>
        /// Dönem başı stok miktarı
        /// </summary>
        public int FirstQuentity { get; set; } = default;

        /// <summary>
        /// Devir hızı
        /// </summary>
        public double RevolutionSpeed { get; set; } = default;

        /// <summary>
        /// Günlük stok durmu
        /// </summary>
        public double DailyStock { get; set; } = default;
        /// <summary>
        /// Günlük stok değişim durumu
        /// </summary>
        public double DailyStockChange { get; set; } = default;
        /// <summary>
        /// Haftalık stok durumu
        /// </summary>
        public double WeeklyStock { get; set; } = default;
        /// <summary>
        /// Haftalık stok değişim durumu
        /// </summary>
        public double WeeklyStockChange { get; set; } = default;
        /// <summary>
        /// Aylık stok durumu
        /// </summary>
        public double MonthlyStock { get; set; } = default;
        /// <summary>
        /// Aylık stok değişim durumu
        /// </summary>
        public double MonthlyStockChange { get; set; } = default;
        /// <summary>
        /// Yıllık stok durumu
        /// </summary>
        public double YearlyStock { get; set; } = default;
        /// <summary>
        /// Yıllık stok değişim durumu
        /// </summary>
        public double YearlyStockChange { get; set; } = default;
    }
}
