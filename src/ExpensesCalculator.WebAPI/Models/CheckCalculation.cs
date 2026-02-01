using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Models;

public class CheckCalculation
{
    public CheckDto Check { get; set; }
    public ICollection<ItemCalculation> Items { get; set; }
    public decimal SumPerParticipant { get; set; }
}
