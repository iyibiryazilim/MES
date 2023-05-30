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

    public float DailyStock { get; set; } = default;

	public float DailyStockChange { get; set; } = default;

	public float WeeklyStock { get; set; } = default;

	public float WeeklyStockChange { get; set; } = default;

	public float MonthlyStock { get; set; } = default;

	public float MonthlyStockChange { get; set; } = default;

	public float YearlyStock { get; set; } = default;

	public float YearlyStockChange { get; set; } = default;
}

