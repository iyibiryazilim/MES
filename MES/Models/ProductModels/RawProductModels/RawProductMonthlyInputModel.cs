namespace MES.Models.ProductModels.RawProductModels
{
    public class RawProductMonthlyInputModel
    {
        public RawProductMonthlyInputModel()
        {
            MonthlyInputValues = new Dictionary<short, double>();
            Months = new List<string>();
        }
        public IDictionary<short, double>? MonthlyInputValues { get; set; }
        public IList<string> Months { get; set; }
    }
}
