using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.Models
{
    public class CheckCalculation
    {
        public Check Check { get; set; }
        public ICollection<ItemCalculation> Items { get; set; } = new List<ItemCalculation>();

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n2}₴")]
        public decimal SumPerParticipant { get; set; }
    }
}
