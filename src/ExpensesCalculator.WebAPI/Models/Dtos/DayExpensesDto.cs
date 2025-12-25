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
    public ICollection<string> Participants { get; set; }
    public string? Location { get; set; }
    public decimal TotalSum { get; set; }
}

public class CreateDayExpensesRequestDto
{
    public DateOnly Date { get; set; }
    public ICollection<string> Participants { get; set; }
    public string? Location { get; set; }
}

public class EditDayExpensesRequestDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public ICollection<string> Participants { get; set; }
    public string? Location { get; set; }
}

public record ShareDayExpensesRequestDto(string NewUserWithAccess);
public record ShareDayExpensesResponseDto(bool IsSuccess, string Error);