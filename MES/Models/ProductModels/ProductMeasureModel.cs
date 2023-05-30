using LBS.Shared.Entity.Models;
using System.ComponentModel;

namespace MES.Models.ProductModels
{
    public class ProductMeasureModel : ProductMeasure
    {
        [DisplayName("Barkod")]
        public string Barcode { get; set; } = string.Empty;

        [DisplayName("Barkod")]
        public string SubunitsetCode { get; set; } = string.Empty;


    }
}
