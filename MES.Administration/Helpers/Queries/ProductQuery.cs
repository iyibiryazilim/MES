﻿using static Android.Content.ClipData;

namespace MES.Administration.Helpers.Queries;

public class ProductQuery
{
    public string ProductListQuery()
    {
        string query = $@"SELECT 
[ReferenceId]=ITEMS.LOGICALREF,
[Code] =ITEMS.CODE,
[Name]=ITEMS.NAME,
[CardType]=ITEMS.CARDTYPE,
[UnitsetReferenceId]=UNITSETF.LOGICALREF,
[UnitsetCode]=UNITSETF.CODE,
[SubUnitsetReferenceId]=UNITSETL.LOGICALREF,
[SubUnitsetCode] =UNITSETL.CODE, [StockQuantity] = (SELECT ISNULL(SUM(ONHAND),0) FROM LV_003_01_STINVTOT WHERE STOCKREF = ITEMS.LOGICALREF AND INVENNO = -1), 
[PlanningQuantity]=(SELECT ISNULL( SUM(PLNAMOUNT),0) FROM LG_003_PRODORD WHERE ITEMREF= ITEMS.LOGICALREF ),
[ActualQuantity]=(SELECT ISNULL (SUM(ACTAMOUNT),0) FROM LG_003_PRODORD WHERE ITEMREF= ITEMS.LOGICALREF )
FROM LG_003_PRODORD AS [PRODORD] WITH (NOLOCK)
LEFT JOIN LG_003_ITEMS AS [ITEMS] WITH (NOLOCK) ON PRODORD.ITEMREF=ITEMS.LOGICALREF
LEFT JOIN LG_003_UNITSETF AS [UNITSETF] WITH (NOLOCK) ON ITEMS.UNITSETREF=UNITSETF.LOGICALREF
LEFT JOIN LG_003_UNITSETL AS [UNITSETL] WITH (NOLOCK) ON UNITSETF.LOGICALREF=UNITSETL.UNITSETREF AND UNITSETL.MAINUNIT=1";
            return query;
    }
}
