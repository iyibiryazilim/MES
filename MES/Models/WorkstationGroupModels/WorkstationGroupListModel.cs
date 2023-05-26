using System.ComponentModel;

namespace MES.Models.WorkstationGroupModels
{
    public class WorkstationGroupListModel
    {
        [DisplayName("Referans Kodu")]
        public int ReferenceId { get; set; } = default;

        [DisplayName("Kodu")]
        public string? Code { get; set; } = string.Empty;

        [DisplayName("Adı")]
        public string? Name { get; set; } = string.Empty;

        [DisplayName("İş istasyonu sayısı")]
        public int WorkstationCount { get; set; } = default;
    }
}
