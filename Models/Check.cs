namespace ExpensesCalculator.Models
{
    public class Check : DbObject
    {
        public ICollection<Item> Items { get; } = new List<Item>();
        public decimal Sum { get; set; }
        public string Location { get; set; }
        public string Payer { get; set; }
        public int DayExpensesId { get; set; }
    }
}
