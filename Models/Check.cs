namespace ExpensesCalculator.Models
{
    public class Check
    {
        public int Id { get; set; }
        public List<Item> Items { get; set; }
        public int Sum { get; set; }
        public string Location { get; set; }
        public string? VerificationPath { get; set; }
    }
}
