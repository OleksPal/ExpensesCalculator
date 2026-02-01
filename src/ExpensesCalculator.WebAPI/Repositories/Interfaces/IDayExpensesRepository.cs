using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Repositories.Interfaces;

public interface IDayExpensesRepository
{
    Task<PagedResultWithDateRangeDto<DayExpenses>> GetAll(string userName, AllDayExpensesRequestDto allDayExpensesRequestDto);
    Task<DayExpenses> GetById(Guid id, string userName);
    Task<Guid> Insert(DayExpenses obj);
    Task Update(DayExpenses obj);
    Task Delete(Guid id, string userName);
}
