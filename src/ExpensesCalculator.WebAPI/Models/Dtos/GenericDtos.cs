namespace ExpensesCalculator.WebAPI.Models.Dtos;

public class PagedResultDto<T>
{
    public ICollection<T> Items { get; set; }
    public int TotalPages { get; set; }
}

public class PagedResultWithDateRangeDto<T> : PagedResultDto<T>
{
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
}
