namespace ExpensesCalculator.Models;

public class Tag : DbObject
{
    public string Name { get; set; }
    public string Color { get; set; }
    public ICollection<ItemTag> ItemTags { get; set; }
}

public class ItemTag
{
    public int ItemId { get; set; }
    public Item Item { get; set; }

    public int TagId { get; set; }
    public Tag Tag { get; set; }
}
