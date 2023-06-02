namespace MES.Models.ProductModels.SemiProductModels
{
    public class SemiProductMonthlyOutputModel
    {
        public SemiProductMonthlyOutputModel()
        {
            MonthlyOutputValues = new Dictionary<short, double>();
            Months = new List<string>();
        }
        public IDictionary<short, double>? MonthlyOutputValues { get; set; }
        public IList<string> Months { get; set; }
    }
}
