namespace ExpensesCalculator.Models
{
    public class Check : DbObject
    {
        public List<Item> Items { get; set; }
        public double Sum { get; set; }
        public string Location { get; set; }
        public string? VerificationPath { get; set; }
        public string Payer { get; set; }
    }
}
