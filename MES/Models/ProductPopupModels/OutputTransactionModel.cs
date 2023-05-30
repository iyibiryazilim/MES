using System;
using System.ComponentModel;

namespace MES.Models.ProductPopupModels
{
    public class OutputTransactionModel
    {
        [Browsable(false)]
        public int ReferenceId { get; set; }

        [DisplayName("Hareket Tarihi")]
        public DateTime? TransactionDate { get; set; } = default;

        [DisplayName("Fiş Referansı")]
        public int? ProductTransactionReferenceId { get; set; } = default;

        [DisplayName("Fiş Numarası")]
        public string? TransactionCode { get; set; } = string.Empty;

        [DisplayName("Hareket Türü")]
        public short? TransactionType { get; set; } = default;

        [DisplayName("Ambar Referansı")]
        public int? WarehouseReferenceId { get; set; } = default;

        [DisplayName("Ambar Numarası")]
        public short? WarehouseNumber { get; set; } = default;

        [DisplayName("Ambar İsmi")]
        public string? WarehouseName { get; set; } = string.Empty;

        [DisplayName("Ürün Miktarı")]
        public double? Quantity { get; set; } = default;

        [DisplayName("Birim Kodu")]
        public string? UnitsetCode { get; set; } = string.Empty;

        [DisplayName("Açıklama")]
        public string? Description { get; set; } = string.Empty;
    }
}

