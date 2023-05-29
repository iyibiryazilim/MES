using System.ComponentModel;

namespace MES.Models.SemiProductModels
{
    public class SemiProductListModel
    {
        public int ReferenceId { get; set; }
        [DisplayName("Kodu")]
        public string Code { get; set; } = string.Empty;
        [DisplayName("Adı")]
        public string Name { get; set; } = string.Empty;
        public short? CardType { get; set; }
        [DisplayName("Ana Birim")]
        public string Unitset { get; set; } = string.Empty;
        [DisplayName("Üretici Kodu")]
        public string ProducerCode { get; set; } = string.Empty;
        [DisplayName("Özel Kod")]
        public string SpeCode { get; set; } = string.Empty;


        public double StockQuantity { get; set; } = default;


        public DateTimeOffset? LastTransactionDate { get; set; } = DateTime.Now;

        [DisplayName("Alım Miktarı")]
        public double PurchaseQuantity { get; set; } = default;

        [DisplayName("Satış Miktarı")]
        public double SalesQuentity { get; set; } = default;

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
