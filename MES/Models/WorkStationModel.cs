using LBS.Shared.Entity.Models;

namespace MES.Models;

public class WorkStationModel : Workstation
{
	public DateTime EstimatedMaintanceDate { get; set; } = default;
	public int RealisationRate { get; set; } = default;


}

