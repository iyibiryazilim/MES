namespace MES.Models.ProductModels.SemiProductModels
{
    public class SemiProductMonthlyInputModel
    {
        public SemiProductMonthlyInputModel()
        {
            MonthlyInputValues = new Dictionary<short, double>();
            Months = new List<string>();
        }
        public IDictionary<short, double>? MonthlyInputValues { get; set; }
        public IList<string> Months { get; set; }
    }
}
