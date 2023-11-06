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

        var result = await Database.CreateTableAsync<YTT.Gateway.Model.Models.WorkOrderModels.ProductionWorkOrderList>();
    }
}
