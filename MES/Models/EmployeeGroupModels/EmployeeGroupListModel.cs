using System.ComponentModel;

namespace MES.Models.EmployeeGroupModels
{
    public class EmployeeGroupListModel
    {
        [DisplayName("Referans Kodu")]
        public int ReferenceId { get; set; } = default;

        [DisplayName("Kodu")]
        public string? Code { get; set; } = string.Empty;

        [DisplayName("Adı")]
        public string? Name { get; set; } = string.Empty;

        [DisplayName("Çalışan sayısı")]
        public int EmployeeCount { get; set; } = default;
    }
}
