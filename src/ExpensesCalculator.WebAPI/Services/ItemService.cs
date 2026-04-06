using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;

namespace ExpensesCalculator.WebAPI.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly ICheckRepository _checkRepository;
    private readonly ITotalSumCalculationService _totalSumCalculationService;
    private readonly IDayExpensesRepository _dayExpensesRepository;

    public ItemService(IItemRepository itemRepository, ICheckRepository checkRepository, ITotalSumCalculationService totalSumCalculationService, IDayExpensesRepository dayExpensesRepository)
    {
        _itemRepository = itemRepository;
        _checkRepository = checkRepository;
        _totalSumCalculationService = totalSumCalculationService;
        _dayExpensesRepository = dayExpensesRepository;
    }

    public async Task<ItemDto?> GetById(Guid id)
    {
        var item = await _itemRepository.GetById(id);

        if (item is null)
            return null;

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Comment = item.Comment,
            Price = item.Price,
            Amount = item.Amount,
            Rating = item.Rating,
            Tags = item.Tags,
            Users = item.Users,
            CheckId = item.CheckId
        };
    }

    public async Task<Item[]> GetAllCheckItems(Guid checkId)
    {
        return await _itemRepository.GetAllCheckItems(checkId);
    }

    public async Task<ItemUpdateResponseDto> AddItem(CreateItemRequestDto itemDto)
    {
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = itemDto.Name,
            Comment = itemDto.Comment,
            Price = itemDto.Price,
            Amount = itemDto.Amount,
            Rating = itemDto.Rating,
            Tags = itemDto.Tags,
            Users = itemDto.Users,
            CheckId = itemDto.CheckId
        };

        await _itemRepository.Insert(item);
        var check = await _checkRepository.GetById(item.CheckId);
        await _totalSumCalculationService.UpdateDayExpensesTotalSum(check.DayExpensesId);
        var checkTotalSum = await _totalSumCalculationService.GetCheckTotalSum(item.CheckId);

        var dayExpenses = await _dayExpensesRepository.GetByIdInternal(check.DayExpensesId);
        var dayExpensesTotalSum = dayExpenses?.TotalSum ?? 0m;

        return new ItemUpdateResponseDto
        {
            Id = item.Id,
            Name = item.Name,
            Comment = item.Comment,
            Price = item.Price,
            Amount = item.Amount,
            Rating = item.Rating,
            Tags = item.Tags,
            Users = item.Users,
            CheckId = item.CheckId,
            CheckTotalSum = checkTotalSum,
            DayExpensesTotalSum = dayExpensesTotalSum
        };
    }

    public async Task<ItemUpdateResponseDto> EditItem(EditItemRequestDto itemDto)
    {
        var item = await _itemRepository.GetById(itemDto.Id);
        if (item == null)
            throw new KeyNotFoundException($"Item with id {itemDto.Id} not found.");

        item.Name = itemDto.Name;
        item.Comment = itemDto.Comment;
        item.Price = itemDto.Price;
        item.Amount = itemDto.Amount;
        item.Rating = itemDto.Rating;
        item.Tags = itemDto.Tags;
        item.Users = itemDto.Users;

        await _itemRepository.Update(item);
        var check = await _checkRepository.GetById(item.CheckId);
        await _totalSumCalculationService.UpdateDayExpensesTotalSum(check.DayExpensesId);
        var checkTotalSum = await _totalSumCalculationService.GetCheckTotalSum(item.CheckId);

        var dayExpenses = await _dayExpensesRepository.GetByIdInternal(check.DayExpensesId);
        var dayExpensesTotalSum = dayExpenses?.TotalSum ?? 0m;

        return new ItemUpdateResponseDto
        {
            Id = item.Id,
            Name = item.Name,
            Comment = item.Comment,
            Price = item.Price,
            Amount = item.Amount,
            Rating = item.Rating,
            Tags = item.Tags,
            Users = item.Users,
            CheckId = item.CheckId,
            CheckTotalSum = checkTotalSum,
            DayExpensesTotalSum = dayExpensesTotalSum
        };
    }

    public async Task<DeleteItemResponse> DeleteItem(Guid id)
    {
        var item = await _itemRepository.GetById(id);
        var checkId = item.CheckId;

        await _itemRepository.Delete(id);
        var check = await _checkRepository.GetById(checkId);
        await _totalSumCalculationService.UpdateDayExpensesTotalSum(check.DayExpensesId);
        var checkTotalSum = await _totalSumCalculationService.GetCheckTotalSum(checkId);

        var dayExpenses = await _dayExpensesRepository.GetByIdInternal(check.DayExpensesId);
        var dayExpensesTotalSum = dayExpenses?.TotalSum ?? 0m;

        return new DeleteItemResponse(checkTotalSum, dayExpensesTotalSum);
    }

}
