using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MES.Client.Databases.SQLiteDatabase.Models;

public class WorkOrder : INotifyPropertyChanged
{
	#region Fields
	int referenceId;
	DateTime date;
	string workOrderCode;
	string workStationCode;
	bool isIntegrated;
	#endregion

	

	private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}


	public event PropertyChangedEventHandler? PropertyChanged;
}
