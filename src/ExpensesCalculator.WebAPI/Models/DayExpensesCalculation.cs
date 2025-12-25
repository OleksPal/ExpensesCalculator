namespace ExpensesCalculator.WebAPI.Models;

public class DayExpensesCalculation
{
    public string UserName { get; set; }
    public ICollection<CheckCalculation> CheckCalculations { get; set; }
}
