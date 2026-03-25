using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.WebAPI.Models.Dtos;

public class ItemDto : Item { }

public class ItemUpdateResponseDto : ItemDto
{
    public decimal CheckTotalSum { get; set; }
    public decimal DayExpensesTotalSum { get; set; }
}

public class CreateItemRequestDto
{
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(1, ErrorMessage = "Name should contain at least one symbol.")]
    public string Name { get; set; }
    public string? Comment { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price should be > 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Amount should be > 0.")]
    public int Amount { get; set; }

    [Required(ErrorMessage = "Rating is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Rating should be > 0.")]
    public int Rating { get; set; }

    public string[] Tags { get; set; }

    [Required(ErrorMessage = "Users is required.")]
    [MinLength(1, ErrorMessage = "Users should be at least 1.")]
    public string[] Users { get; set; }

    [Required(ErrorMessage = "CheckId is required.")]
    public Guid CheckId { get; set; }
}

public class EditItemRequestDto
{
    [Required(ErrorMessage = "Id is required.")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MinLength(1, ErrorMessage = "Name should contain at least one symbol.")]
    public string Name { get; set; }
    public string? Comment { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price should be > 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Amount should be > 0.")]
    public int Amount { get; set; }

    [Required(ErrorMessage = "Rating is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Rating should be > 0.")]
    public int Rating { get; set; }

    public string[] Tags { get; set; }

    [Required(ErrorMessage = "Users is required.")]
    [MinLength(1, ErrorMessage = "Users should be at least 1.")]
    public string[] Users { get; set; }
}

public record DeleteItemResponse(decimal CheckTotalSum, decimal DayExpensesTotalSum);
