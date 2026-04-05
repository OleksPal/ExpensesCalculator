using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Services.Interfaces;

public interface IExpensesCalculator
{
    Task<DayExpensesCalculationsDto> GetCalculations(DayExpensesResponseDto dayExpenses);
}
