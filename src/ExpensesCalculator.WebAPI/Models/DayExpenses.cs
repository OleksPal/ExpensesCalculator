namespace ExpensesCalculator.WebAPI.Models;

public class DayExpenses
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public ICollection<string> Participants { get; set; }
    public ICollection<string> PeopleWithAccess { get; set; }
    public string? Location { get; set; }
}
