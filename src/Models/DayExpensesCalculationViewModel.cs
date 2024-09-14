namespace ExpensesCalculator.Models
{
    public class DayExpensesCalculationViewModel
    {
        public Guid DayExpensesId { get; set; }
        public IEnumerable<string> Participants { get; set; }
        public IEnumerable<Check> Checks { get; set; }
        public ICollection<DayExpensesCalculation> DayExpensesCalculations { get; set; }
        public ICollection<Transaction> AllUsersTrasactions { get; set; }
        public ICollection<Transaction> OptimizedUserTransactions { get; set; }
    }
}
