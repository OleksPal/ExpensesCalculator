namespace ExpensesCalculator.WebAPI.Models;

public class DayExpenses
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public string[] Participants { get; set; }
    public string[] PeopleWithAccess { get; set; }
    public string? Location { get; set; }
    public decimal TotalSum { get; set; }
}
