namespace ExpensesCalculator.WebAPI.Models;

public class Check
{
    public Guid Id { get; set; }
    public string Location { get; set; }
    public string Payer { get; set; }
    public byte[]? Photo { get; set; }
    public Guid DayExpensesId { get; set; }
}
