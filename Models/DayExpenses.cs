using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExpensesCalculator.Models
{
    public class DayExpenses : DbObject
    {
        public ICollection<Check> Checks { get; } = new List<Check>();

        [Required(ErrorMessage = "Please enter expenses date")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateOnly Date { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please enter participants")]
        public ICollection<string> ParticipantsList { get; set; } = new List<string>();

        [Display(Name = "Participants")]
        public string Participants
        {
            get { return JsonConvert.SerializeObject(ParticipantsList); }
            set 
            {
                var participantsList = JsonConvert.DeserializeObject<List<string>>(value);
                ParticipantsList = participantsList.IsNullOrEmpty() ? new List<string>() : participantsList;
            }
        }

        [NotMapped]
        [Required(ErrorMessage = "Please enter people with access")]
        public ICollection<string> PeopleWithAccessList { get; set; } = new List<string>();

        [Display(Name = "People with access")]
        public string PeopleWithAccess
        {
            get { return JsonConvert.SerializeObject(PeopleWithAccessList); }
            set
            {
                var peopleWithAccessList = JsonConvert.DeserializeObject<List<string>>(value);
                PeopleWithAccessList = peopleWithAccessList.IsNullOrEmpty() ? new List<string>() : peopleWithAccessList;
            }
        }
    }
}
