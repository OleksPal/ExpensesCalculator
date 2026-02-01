namespace ExpensesCalculator.WebAPI.Models.Dtos;

public class ItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Amount { get; set; }
    public int Rating { get; set; }
    public ICollection<string> Tags { get; set; }
    public ICollection<string> Users { get; set; }
    public Guid CheckId { get; set; }
}

public class RecommendationItemDto : ItemDto
{
    public bool CanEdit { get; set; }
    public Guid DayExpensesId { get; set; }
}
