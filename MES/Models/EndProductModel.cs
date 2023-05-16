using LBS.Shared.Entity.Models;
using System.ComponentModel;

namespace MES.Models
{
    public class EndProductModel : EndProduct
    {
        
        public int StockQuentity { get; set; } = default;
        public DateTime LastTransactionDate { get; set; } = default;
        [DisplayName("Alım Miktarı")]
		public int PurchaseQuentity { get; set; } = default;
        [DisplayName("Satış Miktarı")]

		public int SellQuentity { get; set; } = default;
        [DisplayName("Dönem başı stok miktarı")]
        public int FirstQuentity { get; set; } = default;
        [DisplayName("Devir Hızı")]
        public double RevolutionSpeed { get; set; } = default;
        
    }
}
