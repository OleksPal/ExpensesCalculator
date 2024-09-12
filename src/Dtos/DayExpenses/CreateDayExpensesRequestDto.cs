using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.Dtos.DayExpenses
{
    public class CreateDayExpensesRequestDto
    {
        [Required(ErrorMessage = "Please enter expenses date")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateOnly Date { get; set; }

        [Display(Name = "Participant list")]
        [Required(ErrorMessage = "Please enter participants list")]
        public ICollection<string> ParticipantList { get; set; }
    }
}
