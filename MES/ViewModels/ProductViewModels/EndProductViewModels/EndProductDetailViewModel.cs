using System;
using MES.Models;
using MES.Models.EndProductModels;
using System.ComponentModel;
using MES.Models.RawProductModels;

namespace MES.ViewModels.ProductViewModels.EndProductViewModels
{
    public class EndProductDetailViewModel
    {
        public EndProductModel? EndProductModel { get; set; }

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

