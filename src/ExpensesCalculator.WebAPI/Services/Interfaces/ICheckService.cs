using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Services.Interfaces;

public interface ICheckService
{
    Task<CheckDto> GetById(Guid id);
    Task AddCheck(Check check);
    Task UpdateCheck(Check check);
    Task DeleteCheck(Guid id);
    Task<ICollection<CheckDto>> GetAllDayExpensesChecks(Guid dayExpensesId);
    Task<decimal> GetTotalSum(Guid id);
}
