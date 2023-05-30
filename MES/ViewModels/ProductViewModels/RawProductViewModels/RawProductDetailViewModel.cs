using System;
using MES.Models.ProductModels.EndProductModels;
using MES.Models.ProductModels.RawProductModels;

namespace MES.ViewModels.ProductViewModels.RawProductViewModels
{
    public class RawProductDetailViewModel
    {
        public RawProductDetailViewModel()
        {
            RawProductMeasureModel = new List<RawProductMeasureModel>();

        }
        public RawProductModel? RawProductModel { get; set; }

        public IList<RawProductMeasureModel>? RawProductMeasureModel { get; set; }
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

