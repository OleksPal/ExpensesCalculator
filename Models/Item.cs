namespace ExpensesCalculator.Models
{
    public class Item : DbObject
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public List<string> Users { get; set; }
    }
}
