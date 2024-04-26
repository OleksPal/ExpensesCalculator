namespace ExpensesCalculator.Models
{
    public class Item : DbObject
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
    }
}
