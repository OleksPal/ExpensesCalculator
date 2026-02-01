using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;

namespace ExpensesCalculator.WebAPI.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;

    public ItemService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Item> GetById(Guid id)
    {
        return await _itemRepository.GetById(id);
    }

    public async Task<ICollection<Item>> GetAllCheckItems(Guid checkId)
    {
        return await _itemRepository.GetAllCheckItems(checkId);
    }

    public async Task<PagedResultDto<Item>> GetAllUserItems(string userName, AllDayExpensesRequestDto request)
    {
        return await _itemRepository.GetAllUserItems(userName, request);
    }

    public async Task<RecommendationsPagedResultDto<RecommendationItemDto>> GetAllItemsForRecommendations(string userName, AllDayExpensesRequestDto request)
    {
        return await _itemRepository.GetAllItemsForRecommendations(userName, request);
    }

    public async Task<ICollection<string>> GetAllDistinctTags()
    {
        return await _itemRepository.GetAllDistinctTags();
    }

    public async Task UpdateItemRatingAndTags(Guid itemId, int rating, ICollection<string> tags)
    {
        var item = await _itemRepository.GetById(itemId);
        if (item != null)
        {
            item.Rating = rating;
            item.Tags = tags;
            await _itemRepository.Update(item);
        }
    }

    public async Task<decimal> AddItem(Item item)
    {
        await _itemRepository.Insert(item);
        return await GetTotalSum(item.CheckId);
    }

    public async Task<decimal> EditItem(Item item)
    {
        await _itemRepository.Update(item);
        return await GetTotalSum(item.CheckId);
    }

    public async Task<decimal> DeleteItem(Guid id)
    {
        var item = await _itemRepository.GetById(id); // TODO: handle null case

        await _itemRepository.Delete(id);
        return await GetTotalSum(item.CheckId);
    }

    private async Task<decimal> GetTotalSum(Guid checkId)
    {
        var items = await _itemRepository.GetAllCheckItems(checkId);
        return items.Select(item => item.Price * item.Amount).Sum();
    }
}
