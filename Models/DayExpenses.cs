namespace ExpensesCalculator.Models
{
    public class DayExpenses
    {
        public List<Check> Checks { get; set; }
        public DateOnly Date { get; set; }
    }
}
