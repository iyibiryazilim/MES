using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MES.Client.Databases.SQLiteDatabase.Models;

public class WorkOrder
{
	#region Fields

	[PrimaryKey]
	public int ReferenceId { get; set; }
	public DateTime Date { get; set; }
	public string WorkOrderCode { get; set; }
	public string WorkStationCode { get; set; }
	public bool IsIntegrated { get; set; }
	#endregion
}
