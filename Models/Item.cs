namespace ExpensesCalculator.Models
{
    public class Item : DbObject
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public ICollection<string> Users { get; } = new List<string>();
        public int CheckId { get; set; }
    }
}
