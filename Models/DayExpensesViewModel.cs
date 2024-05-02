namespace ExpensesCalculator.Models
{
    public class DayExpensesViewModel
    {
        public int DayExpensesId { get; set; }
        public DateOnly Date { get; set; }
        public List<Check>? Checks { get; set; }
        public List<Item>? Items { get; set; }
    }
}
