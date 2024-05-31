namespace ExpensesCalculator.Models
{
    public class DayExpensesViewModel
    {
        public IEnumerable<DayExpenses> Days { get; set; }
        public DayExpenses Day { get; }
        public Check Check { get; }
        public Item Item { get; }
    }
}
