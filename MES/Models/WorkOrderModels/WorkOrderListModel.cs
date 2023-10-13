using System.ComponentModel;

namespace MES.Models.WorkOrderModels
{
    public class WorkOrderListModel
    {
        [DisplayName("Referans Kodu")]
        public int ReferenceId { get; set; }

        [DisplayName("İş İstasyonu Adı")]
        public string? WorkstationName { get; set; } = string.Empty;

        [DisplayName("İş İstasyonu Kodu")]
        public string? WorkstationCode { get; set; } = string.Empty;

        [DisplayName("İş İstasyonu Refereans Kodu")]
        public int WorkstationId { get; set; } = default;

        [DisplayName("Mamul Adı")]
        public string? ProductName { get; set; } = string.Empty;

        [DisplayName("Mamul Referans Kodu")]
        public int ProductReferenceId { get; set; } = default;

        [DisplayName("Mamul Kodu")]
        public string ProductCode { get; set; } = string.Empty;

        [DisplayName("İş Emri Durumu")]
        public int WorkOrderStatus { get; set; } = default;

        [DisplayName("İş Emri Planlanan Başlama Tarihi")]
        public DateTime PlannedBeginDate { get; set; } = default;

        [DisplayName("İş Emri Planlanan Bitiş Tarihi")]
        public DateTime PlannedDueDate { get; set; } = default;

        [DisplayName("Operasyon Referans Kodu")]
        public int OperationId { get; set; } = default;

        [DisplayName("Operasyo Adı")]
        public string? OperationName { get; set; } = string.Empty;

        [DisplayName("Planlanan Miktar")]
        public double PlannedAmounth { get; set; } = default;

        [DisplayName("Planlanan Miktar")]
        public double ActualAmounth { get; set; } = default;

        [DisplayName("Gerçekleşme Oranı")]

        public double RealizationRate { get; set; } = default;
    }
}
