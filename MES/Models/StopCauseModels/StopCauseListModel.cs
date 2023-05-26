using System.ComponentModel;

namespace MES.Models.StopCauseModels
{
	public class StopCauseListModel
	{
		[DisplayName("Referans Kodu")]
		public int ReferenceId { get; set; } = default;

		[DisplayName("Kod")]
		public string? Code { get; set; } = string.Empty;

		[DisplayName("Açıklama")]
		public string? Description { get; set; } = string.Empty;

		[DisplayName("Durma Süresi")]
		public double StopDuration { get; set; } = default;

		[DisplayName("Durma Sayısı")]
		public int StopCount { get; set; } = default;
	}
}
