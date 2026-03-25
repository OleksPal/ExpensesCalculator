using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.WebAPI.Models.Dtos;

public class RecommendationItemDto : ItemDto
{
    public Guid DayExpensesId { get; set; }
    public bool CanEdit { get; set; }
}

public class AllItemsRequestDto
{
    public string? SortColumn { get; set; } = "rating";
    public string? SortOrder { get; set; } = "desc";
    public string? FilterText { get; set; }
    public string? FilterCriteria { get; set; }
    public string[]? Tags { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IsOnlyMyItems { get; set; } = true;
}

public class CreateRecommendationItemRequestDto
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
}

public class EditRecommendationItemRequestDto
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
}
