namespace MES.Models.ProductModels.RawProductModels
{
    public class RawProductMonthlyOutputModel
    {
        public RawProductMonthlyOutputModel()
        {
            MonthlyOutputValues = new Dictionary<short, double>();
            Months = new List<string>();
        }
        public IDictionary<short, double>? MonthlyOutputValues { get; set; }
        public IList<string> Months { get; set; }
    }
}
