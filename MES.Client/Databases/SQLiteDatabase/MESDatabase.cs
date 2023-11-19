using Android.App;
using MES.Client.Databases.SQLiteDatabase.Models;
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
		if (Database is not null)
			return;
		Database = new SQLiteAsyncConnection(Constants.DbConfiguration.DatabasePath, Constants.DbConfiguration.Flags);
		//if(Database is null)
		//{
		//	Database = new SQLiteAsyncConnection(Constants.DbConfiguration.DatabasePath, Constants.DbConfiguration.Flags);
		//}
		await Database.CreateTableAsync<Models.WorkOrderTable>();
	}

	public async Task<List<WorkOrderTable>> GetItemsAsync()
	{
		await Init();
		return await Database.Table<Models.WorkOrderTable>().ToListAsync();

	}

	public async Task<List<WorkOrderTable>> GetItemsNotIntegratedAsync()
	{
		await Init();
		return await Database.Table<Models.WorkOrderTable>().Where(x => !x.IsIntegrated).ToListAsync();

	}

	public async Task<WorkOrderTable> GetItemAsync(int id)
	{
		await Init();
		return await Database.Table<Models.WorkOrderTable>().Where(x => x.ID == id).FirstOrDefaultAsync();

	}

	public async Task<int> DeleteItemAsync(WorkOrderTable item)
	{
		await Init();
		return await Database.DeleteAsync(item);

	}

	public async Task<int> DeleteAllItemAsync()
	{
		await Init();
		//return await Database.DeleteAllAsync(item);
		return await Database.DeleteAllAsync<Models.WorkOrderTable>();

	}

	public async Task InsertWorkOrderAsync(WorkOrderTable workOrderTable)
	{
		await Init();
		if(workOrderTable.ID == default)
		{
			await Database.InsertAsync(workOrderTable);
		} else
		{
			await Database.UpdateAsync(workOrderTable);
		}
	}
}
