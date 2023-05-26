using System.ComponentModel;

namespace MES.Models.OperationModels
{
    public class OperationListModel
    {
        [DisplayName("Operasyon Referans Kodu")]
        public int ReferenceId { get; set; } = default;

        [DisplayName("Operasyon Kodu")]
        public string? Code { get; set; } = string.Empty;

        [DisplayName("Operasyon Açıklaması")]
        public string? Description { get; set; } = string.Empty;

        [DisplayName("Aktif İş Emri Sayısı")]
        public int ActiveWorkOrder { get; set; } = default;


    }
}