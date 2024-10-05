using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.Dtos.User
{
    public class LoginWithUsernameDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
