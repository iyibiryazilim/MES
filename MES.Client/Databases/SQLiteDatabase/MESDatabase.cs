using Android.App;
using SQLite;

namespace MES.Client.Databases.SQLiteDatabase;

public class MESDatabase
{
	SQLiteAsyncConnection Database;

	public MESDatabase()
	{

	}

	public async Task Init()
	{
		//if (Database is not null)
		//	return;
		Database = new SQLiteAsyncConnection(Constants.DbConfiguration.DatabasePath, Constants.DbConfiguration.Flags);
		if(Database is null)
		{
			Database = new SQLiteAsyncConnection(Constants.DbConfiguration.DatabasePath, Constants.DbConfiguration.Flags);
		}
		await Database.CreateTableAsync<Models.WorkOrder>();
	}

	public async Task InsertWorkOrderAsync(Shared.Entity.Models.WorkOrder workOrder)
	{
		SQLiteAsyncConnection Database = new SQLiteAsyncConnection(Constants.DbConfiguration.DatabasePath, Constants.DbConfiguration.Flags);
		//await Init();
		Models.WorkOrder workOrderModel = new Models.WorkOrder();
		//workOrderModel.ID = workOrder.CurrentReferenceId;
		workOrderModel.ReferenceId = workOrder.ReferenceId;
		workOrderModel.Date = DateTime.Now;
		workOrderModel.ProductCode = workOrder.ProductCode;
		workOrderModel.WorkStationCode = workOrder.WorkstationCode;
		workOrderModel.IsIntegrated = true;


		await Database.InsertAsync(workOrderModel);
		
	}
}
