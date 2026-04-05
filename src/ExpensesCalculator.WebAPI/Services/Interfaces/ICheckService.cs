using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Services.Interfaces;

public interface ICheckService
{
    Task<CheckDto> GetById(Guid id);
    Task<CheckDto> AddCheck(CreateCheckRequestDto check);
    Task<CheckDto> UpdateCheck(EditCheckRequestDto check);
    Task<DeleteCheckResponse> DeleteCheck(Guid id);
    Task<CheckDto[]> GetAllDayExpensesChecks(Guid dayExpensesId);
}
