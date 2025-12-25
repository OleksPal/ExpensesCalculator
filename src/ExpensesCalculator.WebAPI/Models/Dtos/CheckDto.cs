namespace ExpensesCalculator.WebAPI.Models.Dtos;

public class CheckDto
{
    public Guid Id { get; set; }
    public string Location { get; set; }
    public string Payer { get; set; }
    public byte[]? Photo { get; set; }
    public Guid DayExpensesId { get; set; }
    public decimal TotalSum { get; set; }
}
