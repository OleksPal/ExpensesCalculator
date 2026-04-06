using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.WebAPI.Models.Dtos;

public class AllDayExpensesRequestDto
{
    public string? SortColumn { get; set; } = "date";
    public string? SortOrder { get; set; } = "desc";
    public string? FilterText { get; set; }
    public string? FilterCriteria { get; set; }
    public DateOnly? FromDate { get; set; } = null;
    public DateOnly? ToDate { get; set; } = null;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class DayExpensesResponseDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public string[] Participants { get; set; }
    public string? Location { get; set; }
    public decimal TotalSum { get; set; }
}

public class DayExpensesDetailsResponseDto : DayExpensesResponseDto
{
    public CheckDto[] Checks { get; set; }
}

public class CreateDayExpensesRequestDto
{
    [Required(ErrorMessage = "Date is required.")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Participants are required.")]
    [MinLength(1, ErrorMessage = "At least one participant is required.")]
    public List<string> Participants { get; set; }

    public string? Location { get; set; }
}

public class EditDayExpensesRequestDto
{
    [Required(ErrorMessage = "Id is required.")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Date is required.")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Participants are required.")]
    [MinLength(1, ErrorMessage = "At least one participant is required.")]
    public List<string> Participants { get; set; }

    public string? Location { get; set; }
}

public record ShareDayExpensesRequestDto(
    [Required(ErrorMessage = "Username is required.")]
    string NewUserWithAccess
);
public record ShareDayExpensesResponseDto(bool IsSuccess, string Error);

public class DayExpensesCalculationsDto
{
    public Guid DayExpensesId { get; set; }
    public string[] Participants { get; set; }
    public ICollection<DayExpensesCalculation> DayExpensesCalculations { get; set; }
    public ICollection<Transaction> AllUsersTransactions { get; set; }
    public ICollection<Transaction> OptimizedUserTransactions { get; set; }
}