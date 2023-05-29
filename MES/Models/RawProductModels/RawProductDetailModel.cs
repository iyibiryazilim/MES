﻿using System.ComponentModel;

namespace MES.Models.RawProductModels
{
    public class RawProductDetailModel
    {
        public RawProductDetailModel()
        {
            ProductMeasures = new List<ProductMeasureModel>();
            WarehouseParameters = new List<ProductWarehouseParameterModel>();
        }
        public RawProductModel? RawProductModel { get; set; }
        public double DailyStock { get; set; } = default;
        [DisplayName("Günlük Stok Miktarı Değişimi")]
        public double DailyStockChange { get; set; } = default;
        [DisplayName("Haftalık Stok Miktarı Toplam")]
        public double WeeklyStock { get; set; }
        [DisplayName("Haftalık Stok Miktarı Değişimi")]
        public double WeeklyStockChange { get; set; } = default;
        [DisplayName("Aylık Stok Miktarı Toplam")]
        public double MonthlyStock { get; set; }
        [DisplayName("Aylık Stok Miktarı Değişimi")]
        public double MonthlyStockChange { get; set; } = default;
        [DisplayName("Yıllık Stok Miktarı Toplam")]
        public double YearlyStock { get; set; }
        [DisplayName("Yıllk Stok Miktarı Değişimi")]
        public double YearlyStockChange { get; set; } = default;

        public IList<ProductMeasureModel>? ProductMeasures { get; set; }
        public IList<ProductWarehouseParameterModel>? WarehouseParameters { get; set; }
    }
}