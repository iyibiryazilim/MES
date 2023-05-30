using LBS.Shared.Entity.Models;
using System.ComponentModel;

namespace MES.Models.ProductPopupModels;

public class WarehouseParameterModel : ProductWarehouseParameter
{
    [DisplayName("Stok Miktarı")]
    public double StockQuantity { get; set; } = default;

    [DisplayName("Ambar Adı")]
    public string? WarehouseName { get; set; } = string.Empty;


}
