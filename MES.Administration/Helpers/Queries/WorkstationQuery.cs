namespace MES.Administration.Helpers.Queries;

public class WorkstationQuery
{

    public string WorkstationListQuery(int currentIndex,int pageSize,string searchText,string status)
    {
        string query = $@"
            SELECT
            [ReferenceId] = WORKSTAT.LOGICALREF,
            [Code] = WORKSTAT.CODE,
            [Name]= WORKSTAT.NAME,
            [WorkOrderReferenceId] = DISPLINE.LOGICALREF,
            [Status]=DISPLINE.LINESTATUS,
            [StatusName]= CASE DISPLINE.LINESTATUS WHEN 0 THEN 'Başlamadı'
            WHEN 1 THEN 'Devam Ediyor'
            WHEN 2 THEN 'Durduruldu'
            WHEN 3 THEN 'Tamamlandı'
            WHEN 4 THEN 'Kapandı'
            END,
            [OEE] = 0,
            [Quality] = 0,
            [Performance]=0,
            [Probability]=0
            FROM LG_008_DISPLINE AS [DISPLINE] WITH (NOLOCK)
            LEFT JOIN LG_008_WORKSTAT AS [WORKSTAT] WITH (NOLOCK) ON DISPLINE.WSREF=WORKSTAT.LOGICALREF
            LEFT JOIN LG_008_WSGRPASS AS [WORKSTATIONGROUPASSING] WITH(NOLOCK) ON WORKSTAT.LOGICALREF = WORKSTATIONGROUPASSING.WSREF
            LEFT JOIN LG_008_WSGRPF AS [WORKSTATIONGROUP] WITH(NOLOCK) ON WORKSTATIONGROUPASSING.WSGRPREF = WORKSTATIONGROUP.LOGICALREF
            WHERE (WORKSTAT.CODE LIKE '%{searchText}%' OR WORKSTAT.NAME LIKE '%{searchText}%') {status}
            ORDER BY WORKSTAT.NAME
            OFFSET {currentIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY
";
        return query;
    }

    public string WorkstationFilterQuery(short status)
    {
        string query = $@"SELECT
[ReferenceId] = WORKSTAT.LOGICALREF,
[Code] = WORKSTAT.CODE,
[Name]= WORKSTAT.NAME,
[WorkOrderReferenceId] = DISPLINE.LOGICALREF,
[Status]=DISPLINE.LINESTATUS,
[StatusName]= CASE DISPLINE.LINESTATUS WHEN 0 THEN 'Başlamadı'
WHEN 1 THEN 'Devam Ediyor'
WHEN 2 THEN 'Durduruldu'
WHEN 3 THEN 'Tamamlandı'
WHEN 4 THEN 'Kapandı'
END,
[OEE] = 0,
[Quality] = 0,
[Performance]=0,
[Probability]=0
FROM LG_008_DISPLINE AS [DISPLINE] WITH (NOLOCK)
LEFT JOIN LG_008_WORKSTAT AS [WORKSTAT] WITH (NOLOCK) ON DISPLINE.WSREF=WORKSTAT.LOGICALREF
LEFT JOIN LG_008_WSGRPASS AS [WORKSTATIONGROUPASSING] WITH(NOLOCK) ON WORKSTAT.LOGICALREF = WORKSTATIONGROUPASSING.WSREF
LEFT JOIN LG_008_WSGRPF AS [WORKSTATIONGROUP] WITH(NOLOCK) ON WORKSTATIONGROUPASSING.WSGRPREF = WORKSTATIONGROUP.LOGICALREF 
WHERE DISPLINE.LINESTATUS={status}";
        return query;
    }



}
public class StatusTypes
{
    public const string WithoutStatus = "";
    public const string baslamadı = " AND DISPLINE.LINESTATUS=0 ";
    public const string devam = " AND DISPLINE.LINESTATUS=1";
    public const string durduruldu = " AND DISPLINE.LINESTATUS=2";
    public const string tamamlandı = "AND DISPLINE.LINESTATUS=3";
    public const string kapandı = " AND DISPLINE.LINESTATUS=4";
}


