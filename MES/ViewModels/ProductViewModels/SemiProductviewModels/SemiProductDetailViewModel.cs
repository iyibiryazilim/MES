using MES.Models.ProductModels.SemiProductModels;

namespace MES.ViewModels.ProductViewModels.SemiProductviewModels;

public class SemiProductDetailViewModel
{
    public SemiProductDetailViewModel()
    {
        SemiProductMeasureModel = new List<SemiProductMeasureModel>();

    }
    public SemiProductModel? SemiProductModel { get; set; }

    public IList<SemiProductMeasureModel>? SemiProductMeasureModel { get; set; }

    public float DailyInputStock { get; set; } = default;

    public float DailyInputStockChange { get; set; } = default;

    public float WeeklyInputStock { get; set; } = default;

    public float WeeklyInputStockChange { get; set; } = default;

    public float MonthlyInputStock { get; set; } = default;

    public float MonthlyInputStockChange { get; set; } = default;

    public float YearlyInputStock { get; set; } = default;

    public float YearlyInputStockChange { get; set; } = default;

    public float DailyOutputStock { get; set; } = default;

    public float DailyOutputStockChange { get; set; } = default;

    public float WeeklyOutputStock { get; set; } = default;

    public float WeeklyOutputStockChange { get; set; } = default;

    public float MonthlyOutputStock { get; set; } = default;

    public float MonthlyOutputStockChange { get; set; } = default;

    public float YearlyOutputStock { get; set; } = default;

    public float YearlyOutputStockChange { get; set; } = default;

    public double TotalInput { get; set; } = default;

    public double TotalOutput { get; set; } = default;

    public double InputOutputRate { get; set; } = default;

}

