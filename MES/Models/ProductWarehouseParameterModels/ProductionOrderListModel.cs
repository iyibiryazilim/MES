using System.ComponentModel;

namespace MES.Models.ProductWarehouseParameterModels
{
    public class ProductionOrderListModel
    {
        [DisplayName("Referans Kodu")]
        public int ReferenceId { get; set; } = default;

        [DisplayName("Kodu")]
        public string? Code { get; set; } = string.Empty;

        [DisplayName("Açıklama")]
        public string? Description { get; set; } = string.Empty;

        [DisplayName("Planlanan Başlangıç Zamanı")]
        public DateTimeOffset? PlannedBeginDate { get; set; } = default;

        [DisplayName("Mamul Referans Kodu")]
        public int ProductRefernceId { get; set; } = default;

        [DisplayName("Mamul Kodu")]
        public string? ProductCode { get; set; } = string.Empty;

        [DisplayName("Mamul Adı")]
        public string? ProductName { get; set; } = string.Empty;

        [DisplayName("Subunitset")]
        public string? SubUnitset { get; set; } = string.Empty;

        [DisplayName("Unitset")]
        public string? Unitset { get; set; } = string.Empty;

        [DisplayName("Planlanan Miktar")]
        public double PlannedAmounth { get; set; } = default;

        [DisplayName("Anlık Miktar")]
        public double ActualAmounth { get; set; } = default;

        [DisplayName("Anlık Miktar")]
        public double RealizationRate { get; set; } = default;
    }
}