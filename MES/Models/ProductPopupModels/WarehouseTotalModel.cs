using System;
using System.ComponentModel;

namespace MES.Models.ProductPopupModels;

public class WarehouseTotalModel
{
    [Browsable(false)]
    public int ReferenceId { get; set; }

    [DisplayName("Son Hareket Tarihi")]
    public DateTimeOffset? LastTransactionDate { get; set; } = default;

    [DisplayName("Ambar Numarası")]
    public short? WarehouseNumber { get; set; } = default;

    [DisplayName("Ambar İsmi")]
    public string? WarehouseName { get; set; } = string.Empty;

    [DisplayName("Stok Miktarı")]
    public double? StockQuantity { get; set; } = default;
}

