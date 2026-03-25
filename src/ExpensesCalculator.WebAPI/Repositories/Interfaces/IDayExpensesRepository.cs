using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Repositories.Interfaces;

public interface IDayExpensesRepository
{
    Task<PagedResultWithDateRangeDto<DayExpenses>> GetAll(string userName, AllDayExpensesRequestDto allDayExpensesRequestDto);
    Task<DayExpenses?> GetById(Guid id, string userName);
    Task<DayExpenses?> GetByIdInternal(Guid id);
    Task<Guid> Insert(DayExpenses dayExpenses);
    Task Update(DayExpenses dayExpenses);
    Task Delete(DayExpenses dayExpenses);
}
