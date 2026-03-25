using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Services.Interfaces;

public interface IDayExpensesService
{
    Task<PagedResultWithDateRangeDto<DayExpensesResponseDto>> GetAllDays(string userName, AllDayExpensesRequestDto allDayExpensesRequestDto);
    Task<DayExpensesResponseDto?> GetById(Guid id, string userName);
    Task<DayExpensesDetailsResponseDto?> GetDayExpensesDetails(Guid id, string userName);
    Task<DayExpensesResponseDto> AddDayExpenses(CreateDayExpensesRequestDto dayExpensesRequestDto, string userName);
    Task<DayExpensesResponseDto> EditDayExpenses(EditDayExpensesRequestDto dayExpensesRequestDto, string userName);
    Task DeleteDayExpenses(Guid dayExpensesId, string userName);
    Task<ShareDayExpensesResponseDto> ShareDayExpenses(Guid id, string newUserWithAccess, string userName);
    Task<DayExpensesCalculationsDto> GetDayExpensesCalculations(Guid id, string userName);
}
