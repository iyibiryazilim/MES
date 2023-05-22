using LBS.Shared.Entity.Models;
using System.ComponentModel;

namespace MES.Models
{
    public class RawProductModel : RawProduct
    {
        public double StockQuantity { get; set; } = default;
        public DateTime LastTransactionDate { get; set; } = default;
        [DisplayName("Alım Miktarı")]
        public double PurchaseQuantity { get; set; } = default;
        [DisplayName("Satış Miktarı")]
        public double SellQuentity { get; set; } = default;
        [DisplayName("Giriş Miktarı")]
        public double InputQuantity { get; set; } = default;
        [DisplayName("Çıkış Miktarı")]
        public double OutputQuantity { get; set; } = default;
        [DisplayName("Dönem başı stok miktarı")]
        public double FirstQuantity { get; set; } = default;
        [DisplayName("Devir Hızı")]
        public double RevolutionSpeed { get; set; } = default;
    }
}
