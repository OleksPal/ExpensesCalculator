using ExpensesCalculator.Models;
using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.ViewModels
{
    public class AddItemViewModel<T>
    {
        [Required(ErrorMessage = "Please enter item name")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        [StringLength(30, ErrorMessage = "Name can have a max of 30 characters")]
        public string? Name { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        [StringLength(100, ErrorMessage = "Description can have a max of 100 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please enter item price")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n2}₴")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter correct price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please enter item amount")]
        [Display(Name = "Amount")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter correct amount")]
        public int Amount { get; set; } = 1;

        [Required(ErrorMessage = "Please enter item rating")]
        [Display(Name = "Rating")]
        [Range(0, 10, ErrorMessage = "Please enter correct rating")]
        public int Rating { get; set; } = 0;

        public ICollection<ItemTag>? ItemTags { get; set; }

        [Required]
        [Display(Name = "Users")]
        public List<string> UserList { get; set; }

        [Required]
        public T CheckId { get; set; }
    }
}
