using MES.Models.ProductModels.EndProductModels;

namespace MES.ViewModels.ProductViewModels.EndProductViewModels
{
    public class EndProductDetailViewModel
    {
        public EndProductDetailViewModel()
        {
            EndProductMeasureModel = new List<EndProductMeasureModel>();
        }
        public EndProductModel? EndProductModel { get; set; }

        public IList<EndProductMeasureModel>? EndProductMeasureModel { get; set; }

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
    }
}

