using System.ComponentModel;

namespace MES.Models.BOM
{
    public class BOMListModel
    {
        [DisplayName("Referans Kodu")]
        public int ReferenceId { get; set; }

        [DisplayName("Kodu")]
        public string? Code { get; set; } = string.Empty;

        [DisplayName("Adı")]
        public string? Name { get; set; } = string.Empty;

        [DisplayName("Revizyon Tarihi")]
        public DateTime RevisionDate { get; set; } = DateTime.Now;

        [DisplayName("Mamul Refereans Kodu")]
        public int ProductReferenceId { get; set; }

        [DisplayName("Mamul Kodu")]
        public string? ProductCode { get; set; } = string.Empty;

        [DisplayName("Mamul Adı")]
        public string? ProductName { get; set; } = string.Empty;

    }
}
