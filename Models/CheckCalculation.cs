namespace ExpensesCalculator.Models
{
    public class CheckCalculation
    {
        public Check Check { get; set; }
        public ICollection<ItemCalculation> Items { get; set; } = new List<ItemCalculation>();
        public decimal SumPerParticipant { get; set; }
    }
}
