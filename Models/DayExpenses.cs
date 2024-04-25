namespace ExpensesCalculator.Models
{
    public class DayExpenses
    {
        public int Id { get; set; }
        public List<Check> Checks { get; set; }
        public DateOnly Date { get; set; }
    }
}
