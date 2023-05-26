using System.ComponentModel;

namespace MES.Models.EmployeeModels
{
    public class EmployeeListModel
    {
        [DisplayName("Referans Kodu")]
        public int ReferenceId { get; set; } = default;

        [DisplayName("Kodu")]
        public string? Code { get; set; } = string.Empty;

        [DisplayName("Adı")]
        public string? Name { get; set; } = string.Empty;

        [DisplayName("Vardiya doluluk oranı")]
        public int ShiftRate { get; set; } = default;
    }
}
