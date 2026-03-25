using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;
using ExpensesCalculator.WebAPI.Models;

namespace ExpensesCalculator.WebAPI.Services;

public class CheckService : ICheckService
{
    private readonly ICheckRepository _checkRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ITotalSumCalculationService _totalSumCalculationService;
    private readonly IDayExpensesRepository _dayExpensesRepository;

    public CheckService(ICheckRepository checkRepository, IItemRepository itemRepository, ITotalSumCalculationService totalSumCalculationService, IDayExpensesRepository dayExpensesRepository)
    {
        _checkRepository = checkRepository;
        _itemRepository = itemRepository;
        _totalSumCalculationService = totalSumCalculationService;
        _dayExpensesRepository = dayExpensesRepository;
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
        checkDto.TotalSum = await _totalSumCalculationService.GetCheckTotalSum(checkDto.Id);

        return checkDto;
    }

    public async Task<CheckDto> AddCheck(CreateCheckRequestDto checkDto)
    {
        var check = new Check
        {
            Id = Guid.NewGuid(),
            Location = checkDto.Location,
            Payer = checkDto.Payer,
            Photo = checkDto.Photo,
            DayExpensesId = checkDto.DayExpensesId
        };

        await _checkRepository.Insert(check);

        var createdCheck = await _checkRepository.GetById(check.Id);
        return new CheckDto
        {
            Id = createdCheck.Id,
            Location = createdCheck.Location,
            Payer = createdCheck.Payer,
            Photo = createdCheck.Photo,
            DayExpensesId = createdCheck.DayExpensesId,
            TotalSum = await _totalSumCalculationService.GetCheckTotalSum(createdCheck.Id)
        };
    }

    public async Task<CheckDto> UpdateCheck(EditCheckRequestDto checkDto)
    {
        var check = await _checkRepository.GetById(checkDto.Id);
        if (check == null)
            throw new KeyNotFoundException($"Check with id {checkDto.Id} not found.");

        check.Location = checkDto.Location;
        check.Payer = checkDto.Payer;

        await _checkRepository.Update(check);

        return new CheckDto
        {
            Id = check.Id,
            Location = check.Location,
            Payer = check.Payer,
            Photo = check.Photo,
            DayExpensesId = check.DayExpensesId,
            TotalSum = await _totalSumCalculationService.GetCheckTotalSum(check.Id)
        };
    }

    public async Task<DeleteCheckResponse> DeleteCheck(Guid id)
    {
        var check = await _checkRepository.GetById(id);

        // Delete all items for this check
        var items = await _itemRepository.GetAllCheckItems(id);
        foreach (var item in items)
        {
            await _itemRepository.Delete(item.Id);
        }

        // Delete the check
        await _checkRepository.Delete(id);

        // Update day expenses total sum
        await _totalSumCalculationService.UpdateDayExpensesTotalSum(check.DayExpensesId);

        var dayExpenses = await _dayExpensesRepository.GetByIdInternal(check.DayExpensesId);
        var dayExpensesTotalSum = dayExpenses?.TotalSum ?? 0m;

        return new DeleteCheckResponse(dayExpensesTotalSum);
    }

    public async Task<CheckDto[]> GetAllDayExpensesChecks(Guid dayExpensesId)
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

        for (int i = 0; i < dtos.Count; i++)
            dtos[i].TotalSum = await _totalSumCalculationService.GetCheckTotalSum(dtos[i].Id);

        return dtos.ToArray();
    }

}
