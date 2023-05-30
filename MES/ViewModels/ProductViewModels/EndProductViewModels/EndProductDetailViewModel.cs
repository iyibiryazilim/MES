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

        public float DailyStock { get; set; } = default;

        public float DailyStockChange { get; set; } = default;

        public float WeeklyStock { get; set; } = default;

        public float WeeklyStockChange { get; set; } = default;

        public float MonthlyStock { get; set; } = default;

        public float MonthlyStockChange { get; set; } = default;

        public float YearlyStock { get; set; } = default;

        public float YearlyStockChange { get; set; } = default;
    }
}

