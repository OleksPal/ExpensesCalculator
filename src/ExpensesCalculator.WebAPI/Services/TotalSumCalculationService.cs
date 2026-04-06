using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;

namespace ExpensesCalculator.WebAPI.Services;

public class TotalSumCalculationService : ITotalSumCalculationService
{
    private readonly IItemRepository _itemRepository;
    private readonly ICheckRepository _checkRepository;
    private readonly IDayExpensesRepository _dayExpensesRepository;

    public TotalSumCalculationService(
        IItemRepository itemRepository,
        ICheckRepository checkRepository,
        IDayExpensesRepository dayExpensesRepository)
    {
        _itemRepository = itemRepository;
        _checkRepository = checkRepository;
        _dayExpensesRepository = dayExpensesRepository;
    }

    public async Task<decimal> GetCheckTotalSum(Guid checkId)
    {
        var items = await _itemRepository.GetAllCheckItems(checkId);
        return items.Select(item => item.Price * item.Amount).Sum();
    }

    public async Task UpdateDayExpensesTotalSum(Guid dayExpensesId)
    {
        var checks = await _checkRepository.GetAllDayChecks(dayExpensesId);
        var totalSum = 0m;

        foreach (var check in checks)
        {
            var items = await _itemRepository.GetAllCheckItems(check.Id);
            totalSum += items.Sum(item => item.Price * item.Amount);
        }

        var dayExpenses = await _dayExpensesRepository.GetByIdInternal(dayExpensesId);
        if (dayExpenses != null)
        {
            dayExpenses.TotalSum = totalSum;
            await _dayExpensesRepository.Update(dayExpenses);
        }
    }
}
