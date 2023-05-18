using LBS.Shared.Entity.Models;
using System.ComponentModel;

namespace MES.Models;

public class ProductWarehouseParameterModel : ProductWarehouseParameter
{
    [DisplayName("Stok Miktarı")]
    public double StockQuantity { get; set; } = default;
    

}
