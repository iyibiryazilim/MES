using System.ComponentModel;

namespace MES.Models.ShiftModels
{
	public class ShiftListModel
	{
		[DisplayName("Referans Kodu")]
		public int ReferenceId { get; set; }
		[DisplayName("Kodu")]
		public string? Code { get; set; } = string.Empty;
		[DisplayName("Adı")]
		public string? Name { get; set; } = string.Empty;
	}
}
