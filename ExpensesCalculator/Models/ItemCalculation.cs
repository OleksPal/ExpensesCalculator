using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.Models
{
    public class ItemCalculation
    {
        public Item Item { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n2}₴")]
        public decimal PricePerUser { get; set; }
    }
}
