using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.Models
{
    public class Check : DbObject
    {
        public ICollection<Item> Items { get; } = new List<Item>();

        [Required(ErrorMessage = "Please enter check sum")]
        [DataType(DataType.Currency)]
        [Display(Name = "Sum")]
        [DisplayFormat(DataFormatString = "{0}₴")]
        [Range(0, Double.MaxValue, ErrorMessage = "Please enter correct sum")]
        public decimal Sum { get; set; }

        [Required(ErrorMessage = "Please enter check location")]
        [DataType(DataType.Text)]
        [Display(Name = "Location")]
        [StringLength(100, ErrorMessage = "Location can have a max of 100 characters")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Please enter check payer")]
        [DataType(DataType.Text)]
        [Display(Name = "Payer")]
        [StringLength(50, ErrorMessage = "Payer`s name can have a max of 50 characters")]
        public string Payer { get; set; }

        public int DayExpensesId { get; set; }
    }
}
