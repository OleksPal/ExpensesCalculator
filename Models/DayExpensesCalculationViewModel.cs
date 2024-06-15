namespace ExpensesCalculator.Models
{
    public class DayExpensesCalculationViewModel
    {
        public int DayExpensesId { get; set; }
        public List<string> Participants { get; set; }
        public List<Check> Checks { get; set; }
        public Dictionary<SenderRecipient, double> UserTransactions { get; set; }
    }
}
