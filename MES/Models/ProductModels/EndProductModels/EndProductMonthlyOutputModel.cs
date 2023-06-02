namespace MES.Models.ProductModels.EndProductModels
{
    public class EndProductMonthlyOutputModel
    {
        public EndProductMonthlyOutputModel()
        {
            MonthlyOutputValues = new Dictionary<short, double>();
            Months = new List<string>();
        }
        public IDictionary<short, double>? MonthlyOutputValues { get; set; }
        public IList<string> Months { get; set; }
    }
}
