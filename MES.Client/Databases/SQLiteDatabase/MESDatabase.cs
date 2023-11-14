using MES.Client.Databases.SQLiteDatabase.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MES.Client.Databases.SQLiteDatabase;

public class MESDatabase
{
    SQLiteAsyncConnection Database;

    public MESDatabase()
    {

    }

    async Task Init()
    {
        if (Database is not null)
            return;
        Database = new SQLiteAsyncConnection(Constants.DbConfiguration.DatabasePath, Constants.DbConfiguration.Flags);

        var result = await Database.CreateTableAsync<WorkOrder>();
    }

    public async Task InsertWorkOrderAsync(WorkOrder workOrder)
    {
        await Init();
        await Database.InsertAsync(workOrder);
    }
}
