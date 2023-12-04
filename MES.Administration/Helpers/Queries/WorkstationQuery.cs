﻿namespace MES.Administration.Helpers.Queries;

public class WorkstationQuery
{

    public string WorkstationListQuery()
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
FROM LG_003_DISPLINE AS [DISPLINE] WITH (NOLOCK)
LEFT JOIN LG_003_WORKSTAT AS [WORKSTAT] WITH (NOLOCK) ON DISPLINE.WSREF=WORKSTAT.LOGICALREF
LEFT JOIN LG_003_WSGRPASS AS [WORKSTATIONGROUPASSING] WITH(NOLOCK) ON WORKSTAT.LOGICALREF = WORKSTATIONGROUPASSING.WSREF
LEFT JOIN LG_003_WSGRPF AS [WORKSTATIONGROUP] WITH(NOLOCK) ON WORKSTATIONGROUPASSING.WSGRPREF = WORKSTATIONGROUP.LOGICALREF";
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
FROM LG_003_DISPLINE AS [DISPLINE] WITH (NOLOCK)
LEFT JOIN LG_003_WORKSTAT AS [WORKSTAT] WITH (NOLOCK) ON DISPLINE.WSREF=WORKSTAT.LOGICALREF
LEFT JOIN LG_003_WSGRPASS AS [WORKSTATIONGROUPASSING] WITH(NOLOCK) ON WORKSTAT.LOGICALREF = WORKSTATIONGROUPASSING.WSREF
LEFT JOIN LG_003_WSGRPF AS [WORKSTATIONGROUP] WITH(NOLOCK) ON WORKSTATIONGROUPASSING.WSGRPREF = WORKSTATIONGROUP.LOGICALREF 
WHERE DISPLINE.LINESTATUS={status}";
        return query;
    }



}
