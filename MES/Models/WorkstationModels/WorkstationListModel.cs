using System.ComponentModel;

namespace MES.Models.WorkstationModels
{
	public class WorkstationListModel
	{
		[DisplayName("Referans Kodu")]
		public int ReferenceId { get; set; } = default;

		[DisplayName("Kodu")]
		public string? Code { get; set; } = string.Empty;

		[DisplayName("Adı")]
		public string? Name { get; set; } = string.Empty;

		[DisplayName("Doluluk Oranı")]
		public int FillRate { get; set; } = default;

		[DisplayName("Tahmini Bakımı")]
		public DateTime EstimatedMaintenanceDate { get; set; } = DateTime.Now;
	}
}
