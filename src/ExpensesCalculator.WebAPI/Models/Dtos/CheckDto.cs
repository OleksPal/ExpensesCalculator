using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.WebAPI.Models.Dtos;

public class CheckDto
{
    public Guid Id { get; set; }
    public string Location { get; set; }
    public string Payer { get; set; }
    public byte[]? Photo { get; set; }
    public Guid DayExpensesId { get; set; }
    public decimal TotalSum { get; set; }
}

public class CreateCheckRequestDto
{
    [Required(ErrorMessage = "Location is required.")]
    [MinLength(1, ErrorMessage = "Location should contain at least one symbol.")]
    public string Location { get; set; }

    [Required(ErrorMessage = "Payer is required.")]
    [MinLength(1, ErrorMessage = "Payer should contain at least one symbol.")]
    public string Payer { get; set; }

    public byte[]? Photo { get; set; } = null;

    [Required(ErrorMessage = "DayExpensesId is required.")]
    public Guid DayExpensesId { get; set; }
}

public class EditCheckRequestDto
{
    [Required(ErrorMessage = "Id is required.")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Location is required.")]
    [MinLength(1, ErrorMessage = "Location should contain at least one symbol.")]
    public string Location { get; set; }

    [Required(ErrorMessage = "Payer is required.")]
    [MinLength(1, ErrorMessage = "Payer should contain at least one symbol.")]
    public string Payer { get; set; }

    public byte[]? Photo { get; set; } = null;

    [Required(ErrorMessage = "DayExpensesId is required.")]
    public Guid DayExpensesId { get; set; }
}
