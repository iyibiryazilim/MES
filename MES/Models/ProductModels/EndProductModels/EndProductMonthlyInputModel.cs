namespace MES.Models.ProductModels.EndProductModels
{
    public class EndProductMonthlyInputModel
    {
        public EndProductMonthlyInputModel()
        {
            MonthlyInputValues = new Dictionary<short, double>();
            Months = new List<string>();
        }
        public IDictionary<short, double>? MonthlyInputValues { get; set; }
        public IList<string> Months { get; set; }
    }
}
