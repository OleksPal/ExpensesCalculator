using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpensesCalculator.Models;

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
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n2}₴")]
    [Range(0, Double.MaxValue, ErrorMessage = "Please enter correct price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Please enter item amount")]
    [Display(Name = "Amount")]
    [Range(1, int.MaxValue, ErrorMessage = "Please enter correct amount")]
    public int Amount { get; set; }

    [Required(ErrorMessage = "Please enter item rating")]
    [Display(Name = "Rating")]
    [Range(0, 10, ErrorMessage = "Please enter correct rating")]
    public double Rating { get; set; }

    [NotMapped]
    [Display(Name = "Users")]
    public ICollection<string> UserList { get; set; } = new List<string>();

    public string Users
    {
        get => JsonConvert.SerializeObject(UserList);
        set
        {
            var usersList = JsonConvert.DeserializeObject<List<string>>(value);
            UserList = usersList.IsNullOrEmpty() ? new List<string>() : usersList;
        }
    }

    public int CheckId { get; set; }
}
