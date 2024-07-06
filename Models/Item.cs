using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpensesCalculator.Models
{
    public class Item : DbObject
    {
        [Required(ErrorMessage = "Please enter item name")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        [StringLength(30, ErrorMessage = "Name can have a max of 30 characters")]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        [StringLength(100, ErrorMessage = "Description can have a max of 100 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please enter item price")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0}₴")]
        [Range(0, Double.MaxValue, ErrorMessage = "Please enter correct price")]
        public decimal Price { get; set; }

        [NotMapped]
        [Display(Name = "Users")]
        public ICollection<string> UsersList { get; set; } = new List<string>();

        public string Users
        {
            get { return JsonConvert.SerializeObject(UsersList); }
            set
            {
                var usersList = JsonConvert.DeserializeObject<List<string>>(value);
                UsersList = usersList.IsNullOrEmpty() ? new List<string>() : usersList;
            }
        }

        public int CheckId { get; set; }
    }
}
