using Shared.Entity.Models;
using System.ComponentModel;

namespace MES.Administration.Models.ProductModels;

public class EndProductModel : EndProduct
{
  
   public  double? stockQuantity;
    public double? StockQuantity
    {
        get => stockQuantity;
        set
        {
            stockQuantity = value;
            NotifyPropertyChanged();
        }
    }

    public double? actualQuantity;
    public double? ActualQuantity
    {
        get => actualQuantity;
        set
        {
            actualQuantity = value;
            NotifyPropertyChanged();
        }
    }

    public double? planningQuantity;
    public double? PlanningQuantity
    {
        get => planningQuantity;
        set
        {
            planningQuantity = value;
            NotifyPropertyChanged();
        }
    }








}
