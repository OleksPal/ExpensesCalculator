using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.Models
{
    public class DayExpenses : DbObject
    {
        public ICollection<Check> Checks { get; } = new List<Check>();

        [Required(ErrorMessage = "Please enter expenses date")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateOnly Date { get; set; }

        [Display(Name = "Participants")]
        public ICollection<string> Participants { get; } = new List<string>();

        [Display(Name = "People with access")]
        public ICollection<string> PeopleWithAccess { get; } = new List<string>();
    }
}
