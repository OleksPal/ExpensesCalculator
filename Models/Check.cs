namespace ExpensesCalculator.Models
{
    public class Check : DbObject
    {
        public List<Item> Items { get; set; }
        public int Sum { get; set; }
        public string Location { get; set; }
        public string? VerificationPath { get; set; }
    }
}
