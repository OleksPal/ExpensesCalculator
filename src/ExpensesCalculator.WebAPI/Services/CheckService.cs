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

    public async Task<CheckDto> GetById(Guid id)
    {
        var check = await _checkRepository.GetById(id);

        var checkDto = new CheckDto
        {
            Id = check.Id,
            Location = check.Location,
            Payer = check.Payer,
            Photo = check.Photo,
            DayExpensesId = check.DayExpensesId
        };
        checkDto.TotalSum = await GetTotalSum(checkDto.Id);

        return checkDto;
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

        var dtos = checks.Select(check => new CheckDto
        {
            Id = check.Id,
            Location = check.Location,
            Payer = check.Payer,
            Photo = check.Photo,
            DayExpensesId = check.DayExpensesId
        }).ToList();

        for (int i=0;i<dtos.Count();i++)
            dtos[i].TotalSum = await GetTotalSum(dtos[i].Id);

        return dtos.ToList();
    }

    public async Task<decimal> GetTotalSum(Guid id)
    {
        var items = await _itemRepository.GetAllCheckItems(id);
        return items.Select(item => item.Price * item.Amount).Sum();
    }
}
