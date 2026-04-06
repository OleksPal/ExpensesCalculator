namespace ExpensesCalculator.WebAPI.Services.Interfaces;

public interface ITotalSumCalculationService
{
    Task<decimal> GetCheckTotalSum(Guid checkId);
    Task UpdateDayExpensesTotalSum(Guid dayExpensesId);
}
