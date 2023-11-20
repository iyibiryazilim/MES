using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MES.Client.Databases.SQLiteDatabase.Models;

public class WorkOrderTable
{
	#region Fields
	[PrimaryKey, AutoIncrement]
	public int ID { get; set; }
	public int ReferenceId { get; set; }
	public DateTime Date { get; set; }	
	public double Quantity { get; set; }
	public bool IsIntegrated { get; set; }
	#endregion
}
