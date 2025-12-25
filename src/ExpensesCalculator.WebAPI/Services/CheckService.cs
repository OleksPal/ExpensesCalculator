using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;

namespace ExpensesCalculator.WebAPI.Services;

public class CheckService : ICheckService
{
    private readonly ICheckRepository _checkRepository;
    private readonly IItemRepository _itemRepository;

    public CheckService(ICheckRepository checkRepository, IItemRepository itemRepository)
    {
        _checkRepository = checkRepository;
        _itemRepository = itemRepository;
    }

    public async Task<Check> GetById(Guid id)
    {
        return await _checkRepository.GetById(id);
    }

    public async Task AddCheck(Check check)
    {
        await _checkRepository.Insert(check);
    }

    public async Task UpdateCheck(Check check)
    {
        await _checkRepository.Update(check);
    }

    public async Task DeleteCheck(Guid id)
    {
        await _checkRepository.Delete(id);
    }

    public async Task<ICollection<CheckDto>> GetAllDayExpensesChecks(Guid dayExpensesId)
    {
        var checks = await _checkRepository.GetAllDayChecks(dayExpensesId);

        var tasks = checks.Select(async check => new CheckDto
        {
            Id = check.Id,
            Location = check.Location,
            Payer = check.Payer,
            Photo = check.Photo,
            DayExpensesId = check.DayExpensesId,
            TotalSum = await GetTotalSum(check.Id)
        });

        return await Task.WhenAll(tasks);
    }

    public async Task<decimal> GetTotalSum(Guid id)
    {
        var items = await _itemRepository.GetAllCheckItems(id);
        return items.Select(item => item.Price).Sum();
    }
}
